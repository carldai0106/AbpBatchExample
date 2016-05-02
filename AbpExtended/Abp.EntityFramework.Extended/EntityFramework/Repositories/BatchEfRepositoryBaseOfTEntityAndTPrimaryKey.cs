using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.EntityFramework.Batch;
using Abp.Reflection.Extensions;
using EntityFramework.DynamicFilters;
using EntityFramework.Extensions;
using EfUtils = EntityFramework.Utilities;
using EntityFramework.Mapping;

namespace Abp.EntityFramework.Repositories
{
    public class BatchEfRepositoryBase<TDbContext, TEntity, TPrimaryKey> :
        EfRepositoryBase<TDbContext, TEntity, TPrimaryKey>,
        IBatchRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
        where TDbContext : DbContext
    {
        protected BatchEfRepositoryBase(
            IDbContextProvider<TDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
            IocManager.Instance.RegisterIfNot<IAbpBatchRunner<TDbContext>, AbpSqlServerBatchRunner<TDbContext>>();
        }

        public async Task BatchInsertAsync(IEnumerable<TEntity> entities)
        {
            await Task.Run(() =>
            {
                EfUtils.EFBatchOperation.For(Context, Table).InsertAll(entities);
            });
        }

        public async Task BatchUpdateAsync(IEnumerable<TEntity> entities,
            params Expression<Func<TEntity, object>>[] properties)
        {
            await Task.Run(() =>
            {
                EfUtils.EFBatchOperation.For(Context, Table).UpdateAll(entities, x => x.ColumnsToUpdate(properties));
            });
        }

        public async Task BatchDeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            using (var runner = IocManager.Instance.ResolveAsDisposable<IAbpBatchRunner<TDbContext>>())
            {
                ObjectQuery<TEntity> sourceQuery = Table.Where(predicate).ToObjectQuery();
                EntityMap entityMap = sourceQuery.GetEntityMap<TEntity>();

                if (typeof(TEntity).IsInheritsOrImplements(typeof(ISoftDelete)))
                {
                    var paramExp = Expression.Parameter(typeof(TEntity), "x");
                    var newAnimal = Expression.New(typeof(TEntity));
                    var speciesMember = typeof(TEntity).GetMember("IsDeleted")[0];
                    var speciesMemberBinding = Expression.Bind(speciesMember, Expression.Constant(true, typeof(bool)));
                    var memberInitExpression = Expression.MemberInit(newAnimal, speciesMemberBinding);
                    var lambda = Expression.Lambda<Func<TEntity, TEntity>>(memberInitExpression, paramExp);                   
                    await runner.Object.UpdateAsync(Context, entityMap, sourceQuery, lambda);
                }
                else
                {
                    await runner.Object.DeleteAsync(Context, entityMap, sourceQuery);
                }
            }            
        }

        public async Task BatchUpdateAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> updateExpression)
        {
            using (var runner = IocManager.Instance.ResolveAsDisposable<IAbpBatchRunner<TDbContext>>())
            {
                ObjectQuery<TEntity> sourceQuery = Table.Where(predicate).ToObjectQuery();
                EntityMap entityMap = sourceQuery.GetEntityMap<TEntity>();       

                await runner.Object.UpdateAsync(Context, entityMap, sourceQuery, updateExpression);
            }
        }
    }
}
