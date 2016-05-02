using System;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EntityFramework.Batch;
using EntityFramework.Extensions;
using EntityFramework.Mapping;
using EntityFramework.Reflection;
using System.Data.SqlClient;
using EntityFramework.DynamicFilters;
using Abp.Domain.Uow;

namespace Abp.EntityFramework.Batch
{
    /// <summary>
    /// A batch execution runner for SQL Server.
    /// </summary>
    public class AbpSqlServerBatchRunner<TDbContext> :         
        IAbpBatchRunner<TDbContext> where TDbContext : DbContext
    {
        public Task<int> UpdateAsync<TEntity>(
            TDbContext dbContext, 
            EntityMap entityMap, 
            ObjectQuery<TEntity> query, 
            Expression<Func<TEntity, TEntity>> updateExpression) where TEntity : class
        {
            return InternalUpdate(dbContext, entityMap, query, updateExpression, true);
        }

        private async Task<int> InternalUpdate<TEntity>(TDbContext dbContext, 
            EntityMap entityMap, ObjectQuery<TEntity> query, 
            Expression<Func<TEntity, TEntity>> updateExpression, 
            bool async = false)
            where TEntity : class

        {
            DbConnection updateConnection = null;
            DbTransaction updateTransaction = null;
            DbCommand updateCommand = null;
            var ownConnection = false;
            var ownTransaction = false;

            try
            {
                var objectContext = (dbContext as IObjectContextAdapter).ObjectContext;

                // get store connection and transaction
                var store = GetStore(dbContext);
                updateConnection = store.Item1;
                updateTransaction = store.Item2;

                if (updateConnection.State != ConnectionState.Open)
                {
                    updateConnection.Open();
                    ownConnection = true;
                }

                // use existing transaction or create new
                if (updateTransaction == null)
                {
                    updateTransaction = updateConnection.BeginTransaction();
                    ownTransaction = true;
                }

                updateCommand = updateConnection.CreateCommand();
                updateCommand.Transaction = updateTransaction;
                if (objectContext.CommandTimeout.HasValue)
                    updateCommand.CommandTimeout = objectContext.CommandTimeout.Value;

                var innerSelect = GetSelectSql(dbContext, query, entityMap, updateCommand);
                var sqlBuilder = new StringBuilder(innerSelect.Length * 2);

                sqlBuilder.Append("UPDATE ");
                sqlBuilder.Append(entityMap.TableName);
                sqlBuilder.AppendLine(" SET ");

                var memberInitExpression = updateExpression.Body as MemberInitExpression;
                if (memberInitExpression == null)
                    throw new ArgumentException("The update expression must be of type MemberInitExpression.", "updateExpression");

                int nameCount = 0;
                bool wroteSet = false;
                foreach (MemberBinding binding in memberInitExpression.Bindings)
                {
                    if (wroteSet)
                        sqlBuilder.AppendLine(", ");

                    string propertyName = binding.Member.Name;
                    string columnName = entityMap.PropertyMaps
                        .Where(p => p.PropertyName == propertyName)
                        .Select(p => p.ColumnName)
                        .FirstOrDefault();


                    var memberAssignment = binding as MemberAssignment;
                    if (memberAssignment == null)
                        throw new ArgumentException("The update expression MemberBinding must only by type MemberAssignment.", "updateExpression");

                    Expression memberExpression = memberAssignment.Expression;

                    ParameterExpression parameterExpression = null;
                    memberExpression.Visit((ParameterExpression p) =>
                    {
                        if (p.Type == entityMap.EntityType)
                            parameterExpression = p;

                        return p;
                    });


                    if (parameterExpression == null)
                    {
                        object value;

                        if (memberExpression.NodeType == ExpressionType.Constant)
                        {
                            var constantExpression = memberExpression as ConstantExpression;
                            if (constantExpression == null)
                                throw new ArgumentException(
                                    "The MemberAssignment expression is not a ConstantExpression.", "updateExpression");

                            value = constantExpression.Value;
                        }
                        else
                        {
                            LambdaExpression lambda = Expression.Lambda(memberExpression, null);
                            value = lambda.Compile().DynamicInvoke();
                        }

                        if (value != null)
                        {
                            string parameterName = "p__update__" + nameCount++;
                            var parameter = updateCommand.CreateParameter();
                            parameter.ParameterName = parameterName;
                            parameter.Value = value;
                            updateCommand.Parameters.Add(parameter);

                            sqlBuilder.AppendFormat("[{0}] = @{1}", columnName, parameterName);
                        }
                        else
                        {
                            sqlBuilder.AppendFormat("[{0}] = NULL", columnName);
                        }
                    }
                    else
                    {
                        // create clean objectset to build query from
                        var objectSet = objectContext.CreateObjectSet<TEntity>();

                        Type[] typeArguments = new[] { entityMap.EntityType, memberExpression.Type };

                        ConstantExpression constantExpression = Expression.Constant(objectSet);
                        LambdaExpression lambdaExpression = Expression.Lambda(memberExpression, parameterExpression);

                        MethodCallExpression selectExpression = Expression.Call(
                            typeof(Queryable),
                            "Select",
                            typeArguments,
                            constantExpression,
                            lambdaExpression);

                        // create query from expression
                        var selectQuery = objectSet.CreateQuery(selectExpression, entityMap.EntityType);
                        string sql = selectQuery.ToTraceString();

                        // parse select part of sql to use as update
                        string regex = @"SELECT\s*\r\n\s*(?<ColumnValue>.+)?\s*AS\s*(?<ColumnAlias>\[\w+\])\r\n\s*FROM\s*(?<TableName>\[\w+\]\.\[\w+\]|\[\w+\])\s*AS\s*(?<TableAlias>\[\w+\])";
                        Match match = Regex.Match(sql, regex);
                        if (!match.Success)
                            throw new ArgumentException("The MemberAssignment expression could not be processed.", "updateExpression");

                        string value = match.Groups["ColumnValue"].Value;
                        string alias = match.Groups["TableAlias"].Value;

                        value = value.Replace(alias + ".", "j0.");

                        foreach (ObjectParameter objectParameter in selectQuery.Parameters)
                        {
                            string parameterName = "p__update__" + nameCount++;

                            var parameter = updateCommand.CreateParameter();
                            parameter.ParameterName = parameterName;
                            parameter.Value = objectParameter.Value ?? DBNull.Value;
                            updateCommand.Parameters.Add(parameter);

                            value = value.Replace(objectParameter.Name, parameterName);
                        }
                        sqlBuilder.AppendFormat("[{0}] = {1}", columnName, value);
                    }
                    wroteSet = true;
                }

                sqlBuilder.AppendLine(" ");
                sqlBuilder.AppendFormat("FROM {0} AS j0 INNER JOIN (", entityMap.TableName);
                sqlBuilder.AppendLine();
                sqlBuilder.AppendLine(innerSelect);
                sqlBuilder.Append(") AS j1 ON (");

                bool wroteKey = false;
                foreach (var keyMap in entityMap.KeyMaps)
                {
                    if (wroteKey)
                        sqlBuilder.Append(" AND ");

                    sqlBuilder.AppendFormat("j0.[{0}] = j1.[{0}]", keyMap.ColumnName);
                    wroteKey = true;
                }
                sqlBuilder.Append(")");

                updateCommand.CommandText = sqlBuilder.ToString();

                int result = async
                    ? await updateCommand.ExecuteNonQueryAsync().ConfigureAwait(false)
                    : updateCommand.ExecuteNonQuery();

                // only commit if created transaction
                if (ownTransaction)
                    updateTransaction.Commit();

                return result;
            }
            finally
            {
                if (updateCommand != null)
                    updateCommand.Dispose();
                if (updateTransaction != null && ownTransaction)
                    updateTransaction.Dispose();
                if (updateConnection != null && ownConnection)
                    updateConnection.Close();
            }
        }

        private static Tuple<DbConnection, DbTransaction> GetStore(TDbContext dbContext)
        {
            // TODO, re-eval if this is needed
            var objectContext = (dbContext as IObjectContextAdapter).ObjectContext;

            DbConnection dbConnection = objectContext.Connection;
            var entityConnection = dbConnection as EntityConnection;

            // by-pass entity connection
            if (entityConnection == null)
                return new Tuple<DbConnection, DbTransaction>(dbConnection, null);

            DbConnection connection = entityConnection.StoreConnection;

            // get internal transaction
            dynamic connectionProxy = new DynamicProxy(entityConnection);
            dynamic entityTransaction = connectionProxy.CurrentTransaction;
            if (entityTransaction == null)
                return new Tuple<DbConnection, DbTransaction>(connection, null);

            DbTransaction transaction = entityTransaction.StoreTransaction;
            return new Tuple<DbConnection, DbTransaction>(connection, transaction);
        }

        private static string GetSelectSql<TEntity>(TDbContext dbContext, ObjectQuery<TEntity> query, EntityMap entityMap, DbCommand command)
            where TEntity : class
        {
            // TODO change to esql?

            // changing query to only select keys
            var selector = new StringBuilder(50);
            selector.Append("new(");
            foreach (var propertyMap in entityMap.KeyMaps)
            {
                if (selector.Length > 4)
                    selector.Append((", "));

                selector.Append(propertyMap.PropertyName);
            }
            selector.Append(")");

            var selectQuery = DynamicQueryable.Select(query, selector.ToString());
            var objectQuery = selectQuery as ObjectQuery;

            if (objectQuery == null)
                throw new ArgumentException("The query must be of type ObjectQuery.", "query");

            string innerJoinSql = objectQuery.ToTraceString();

            // create parameters
            foreach (var objectParameter in objectQuery.Parameters)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = objectParameter.Name;
                parameter.Value = objectParameter.Value ?? DBNull.Value;

                command.Parameters.Add(parameter);
            }

            var parameters = new[]
               {
                    new SqlParameter("@DynamicFilterParam_1", SqlDbType.Int),
                    new SqlParameter("@DynamicFilterParam_2", SqlDbType.Bit),
                    new SqlParameter("@DynamicFilterParam_3", SqlDbType.Bit),
                    new SqlParameter("@DynamicFilterParam_4", SqlDbType.Bit)
                };


            var abpContext = (dbContext as AbpDbContext);
            if (abpContext != null)
            {
                parameters[0].Value = abpContext.AbpSession.TenantId;
                if (abpContext.IsFilterEnabled(AbpDataFilters.MayHaveTenant) ||
                    abpContext.IsFilterEnabled(AbpDataFilters.MustHaveTenant))
                {
                    parameters[1].Value = DBNull.Value;
                }
                else
                {
                    parameters[1].Value = 1;
                }

                parameters[2].Value = false;


                if (abpContext.IsFilterEnabled(AbpDataFilters.SoftDelete))
                {
                    parameters[3].Value = DBNull.Value;
                }
                else
                {
                    parameters[3].Value = 1;
                }
            }

            command.Parameters.AddRange(parameters);


            return innerJoinSql;
        }

        public Task<int> DeleteAsync<TEntity>(
            TDbContext dbContext, EntityMap entityMap, 
            ObjectQuery<TEntity> query) where TEntity : class
        {
            return InternalDelete(dbContext, entityMap, query, true);
        }

        private async Task<int> InternalDelete<TEntity>(TDbContext dbContext,
            EntityMap entityMap, ObjectQuery<TEntity> query,
            bool async = false)
            where TEntity : class

        {
            DbConnection deleteConnection = null;
            DbTransaction deleteTransaction = null;
            DbCommand deleteCommand = null;
            bool ownConnection = false;
            bool ownTransaction = false;

            try
            {
                var objectContext = (dbContext as IObjectContextAdapter).ObjectContext;

                // get store connection and transaction
                var store = GetStore(dbContext);
                deleteConnection = store.Item1;
                deleteTransaction = store.Item2;

                if (deleteConnection.State != ConnectionState.Open)
                {
                    deleteConnection.Open();
                    ownConnection = true;
                }

                if (deleteTransaction == null)
                {
                    deleteTransaction = deleteConnection.BeginTransaction();
                    ownTransaction = true;
                }


                deleteCommand = deleteConnection.CreateCommand();
                deleteCommand.Transaction = deleteTransaction;
                if (objectContext.CommandTimeout.HasValue)
                    deleteCommand.CommandTimeout = objectContext.CommandTimeout.Value;

                var innerSelect = GetSelectSql(dbContext, query, entityMap, deleteCommand);

                var sqlBuilder = new StringBuilder(innerSelect.Length * 2);

                sqlBuilder.Append("DELETE ");
                sqlBuilder.Append(entityMap.TableName);
                sqlBuilder.AppendLine();

                sqlBuilder.AppendFormat("FROM {0} AS j0 INNER JOIN (", entityMap.TableName);
                sqlBuilder.AppendLine();
                sqlBuilder.AppendLine(innerSelect);
                sqlBuilder.Append(") AS j1 ON (");

                bool wroteKey = false;
                foreach (var keyMap in entityMap.KeyMaps)
                {
                    if (wroteKey)
                        sqlBuilder.Append(" AND ");

                    sqlBuilder.AppendFormat("j0.[{0}] = j1.[{0}]", keyMap.ColumnName);
                    wroteKey = true;
                }
                sqlBuilder.Append(")");

                deleteCommand.CommandText = sqlBuilder.ToString();

                int result = async
                    ? await deleteCommand.ExecuteNonQueryAsync().ConfigureAwait(false)
                    : deleteCommand.ExecuteNonQuery();

                // only commit if created transaction
                if (ownTransaction)
                    deleteTransaction.Commit();

                return result;
            }
            finally
            {
                if (deleteCommand != null)
                    deleteCommand.Dispose();

                if (deleteTransaction != null && ownTransaction)
                    deleteTransaction.Dispose();

                if (deleteConnection != null && ownConnection)
                    deleteConnection.Close();
            }
        }
    }
}