using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.EntityFramework.Batch;
using Abp.Reflection.Extensions;
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
        public BatchEfRepositoryBase(
            IDbContextProvider<TDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
            IocManager.Instance.RegisterIfNot<IAbpBatchRunner<TDbContext>, AbpSqlServerBatchRunner<TDbContext>>(DependencyLifeStyle.Transient);
        }

        public async Task<IEnumerable<TEntity>> BatchInsertAsync(IEnumerable<TEntity> entities)
        {
            return await Task.FromResult(Table.AddRange(entities));
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

                var paramExp = Expression.Parameter(typeof(TEntity), "x");
                var newEntity = Expression.New(typeof(TEntity));
                var speciesMember = typeof(TEntity).GetMember("IsDeleted")[0];
                var speciesMemberBinding = Expression.Bind(speciesMember, Expression.Constant(true, typeof(bool)));
                var memberInitExpression = Expression.MemberInit(newEntity, speciesMemberBinding);
                var lambda = Expression.Lambda<Func<TEntity, TEntity>>(memberInitExpression, paramExp);
                return await runner.Object.UpdateAsync(Context, entityMap, sourceQuery, lambda);
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

                return await runner.Object.UpdateAsync(Context, entityMap, sourceQuery, updateExpression);
            }
        }
    }
}
