using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Repositories;
using Abp.EntityFramework.Batch;
using Abp.Reflection.Extensions;
using Abp.Runtime.Session;
using Abp.Timing;
using EntityFramework.Extensions;
using EntityFramework.Mapping;

namespace Abp.EntityFramework.Repositories
{
    public class BatchEfRepositoryBase<TDbContext, TEntity, TPrimaryKey> :
        EfRepositoryBase<TDbContext, TEntity, TPrimaryKey>,
        IBatchRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
        where TDbContext : DbContext
    {
        private IAbpSession AbpSession
        {
            get
            {
                var abpDbContext = Context as AbpDbContext;
                return abpDbContext?.AbpSession;
            }
        }

        public BatchEfRepositoryBase(
            IDbContextProvider<TDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
            IocManager.Instance.RegisterIfNot<IAbpBatchRunner<TDbContext>, AbpSqlServerBatchRunner<TDbContext>>(DependencyLifeStyle.Transient);
        }

        public IEnumerable<TEntity> BatchInsert(IEnumerable<TEntity> entities)
        {
            return Table.AddRange(entities);
        }

        public async Task<IEnumerable<TEntity>> BatchInsertAsync(IEnumerable<TEntity> entities)
        {
            return await Task.FromResult(Table.AddRange(entities));
        }

        public int BatchDelete(Expression<Func<TEntity, bool>> predicate)
        {
            using (var runner = IocManager.Instance.ResolveAsDisposable<IAbpBatchRunner<TDbContext>>())
            {
                var sourceQuery = Table.Where(predicate).ToObjectQuery();
                var entityMap = sourceQuery.GetEntityMap<TEntity>();

                if (!typeof(TEntity).IsInheritsOrImplements(typeof(ISoftDelete)))
                {
                    return runner.Object.Delete(Context, entityMap, sourceQuery);
                }

                var lambda = GetDeleteExpression();
                return runner.Object.Update(Context, entityMap, sourceQuery, lambda);
            }
        }

        public async Task<int> BatchDeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            using (var runner = IocManager.Instance.ResolveAsDisposable<IAbpBatchRunner<TDbContext>>())
            {
                var sourceQuery = Table.Where(predicate).ToObjectQuery();
                var entityMap = sourceQuery.GetEntityMap<TEntity>();

                if (!typeof (TEntity).IsInheritsOrImplements(typeof(ISoftDelete)))
                {
                    return await runner.Object.DeleteAsync(Context, entityMap, sourceQuery);
                }

                var lambda = GetDeleteExpression();
                return await runner.Object.UpdateAsync(Context, entityMap, sourceQuery, lambda);
            }
        }

        public int BatchUpdate(
           Expression<Func<TEntity, bool>> predicate,
           Expression<Func<TEntity, TEntity>> updateExpression)
        {
            using (var runner = IocManager.Instance.ResolveAsDisposable<IAbpBatchRunner<TDbContext>>())
            {
                var sourceQuery = Table.Where(predicate).ToObjectQuery();
                var entityMap = sourceQuery.GetEntityMap<TEntity>();

                updateExpression = GetUpdateExpression(updateExpression);
                return runner.Object.Update(Context, entityMap, sourceQuery, updateExpression);
            }
        }

        public async Task<int> BatchUpdateAsync(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TEntity>> updateExpression)
        {
            using (var runner = IocManager.Instance.ResolveAsDisposable<IAbpBatchRunner<TDbContext>>())
            {
                var sourceQuery = Table.Where(predicate).ToObjectQuery();
                var entityMap = sourceQuery.GetEntityMap<TEntity>();

                updateExpression = GetUpdateExpression(updateExpression);

                return await runner.Object.UpdateAsync(Context, entityMap, sourceQuery, updateExpression);
            }
        }

        private Expression<Func<TEntity, TEntity>> GetUpdateExpression(Expression<Func<TEntity, TEntity>> updateExpression)
        {
            var list = new List<MemberAssignment>();

            var pars = updateExpression.Parameters;
            if (pars.Count != 1)
            {
                throw new ArgumentException("The number of parameters of update expression allows only one.",
                    nameof(updateExpression));
            }

            var parameterName = pars.ElementAt(0).Name;
            var paramExp = Expression.Parameter(typeof(TEntity), parameterName);
            var newEntity = Expression.New(typeof(TEntity));

            if (typeof(TEntity).IsInheritsOrImplements(typeof(IHasModificationTime)))
            {
                var propLastModificationTime = typeof(TEntity).GetProperty("LastModificationTime");
                if (propLastModificationTime != null)
                {
                    var bindLastModificationTime = Expression.Bind(propLastModificationTime,
                        Expression.Constant(Clock.Now, propLastModificationTime.PropertyType));
                    list.Add(bindLastModificationTime);
                }
            }

            if (AbpSession?.UserId != null && typeof(TEntity).IsInheritsOrImplements(typeof(IModificationAudited)))
            {
                var propLastModifierUserId = typeof(TEntity).GetProperty("LastModifierUserId");
                if (propLastModifierUserId != null)
                {
                    var bindLastModifierUserId = Expression.Bind(propLastModifierUserId,
                        Expression.Constant(AbpSession?.UserId, propLastModifierUserId.PropertyType));
                    list.Add(bindLastModifierUserId);
                }
            }

            var memberUpdateExpression = updateExpression.Body as MemberInitExpression;
            if (memberUpdateExpression == null)
            {
                throw new ArgumentException("The update expression must be of type MemberInitExpression.",
                    nameof(updateExpression));
            }

            //Get all MemeberAssignment from updateExpression and Merged updateMemeber to new List.
            var updateMemebers = memberUpdateExpression.Bindings.Cast<MemberAssignment>().ToList();
            list.AddRange(updateMemebers);

            //Merged MemeberAssignment and inital it.
            var memberInitExpression = Expression.MemberInit(newEntity, list);
            var lambda = Expression.Lambda<Func<TEntity, TEntity>>(memberInitExpression, paramExp);

            return lambda;
        }

        private Expression<Func<TEntity, TEntity>> GetDeleteExpression()
        {
            var list = new List<MemberAssignment>();
            var paramExp = Expression.Parameter(typeof(TEntity), "x");
            var newEntity = Expression.New(typeof(TEntity));

            var propIsDeleted = typeof(TEntity).GetProperty("IsDeleted");
            if (propIsDeleted != null)
            {
                var bindIsDeleted = Expression.Bind(propIsDeleted, Expression.Constant(true, propIsDeleted.PropertyType));
                list.Add(bindIsDeleted);
            }

            if (typeof(TEntity).IsInheritsOrImplements(typeof(IHasDeletionTime)))
            {
                var propDeletionTime = typeof(TEntity).GetProperty("DeletionTime");
                if (propDeletionTime != null)
                {
                    var bindDeletionTime = Expression.Bind(propDeletionTime,
                        Expression.Constant(Clock.Now, propDeletionTime.PropertyType));
                    list.Add(bindDeletionTime);
                }
            }

            if (AbpSession?.UserId != null && typeof(TEntity).IsInheritsOrImplements(typeof(IDeletionAudited)))
            {
                var propDeleterUserId = typeof(TEntity).GetProperty("DeleterUserId");
                if (propDeleterUserId != null)
                {
                    var bindDeleterUserId = Expression.Bind(propDeleterUserId,
                        Expression.Constant(AbpSession?.UserId, propDeleterUserId.PropertyType));
                    list.Add(bindDeleterUserId);
                }
            }

            var memberInitExpression = Expression.MemberInit(newEntity, list);
            var lambda = Expression.Lambda<Func<TEntity, TEntity>>(memberInitExpression, paramExp);
            return lambda;
        }
    }
}
