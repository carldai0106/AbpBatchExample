using System;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EntityFramework.Mapping;
using System.Data.Entity.Core.Objects;

namespace Abp.EntityFramework.Batch
{
    public interface IAbpBatchRunner<in TDbContext>  where TDbContext : DbContext
    {
        int Update<TEntity>(
           TDbContext dbContext,
           EntityMap entityMap,
           ObjectQuery<TEntity> query,
           Expression<Func<TEntity, TEntity>> updateExpression)
           where TEntity : class;

        Task<int> UpdateAsync<TEntity>(
            TDbContext dbContext, 
            EntityMap entityMap, 
            ObjectQuery<TEntity> query, 
            Expression<Func<TEntity, TEntity>> updateExpression)
            where TEntity : class;

        int Delete<TEntity>(
           TDbContext dbContext,
           EntityMap entityMap,
           ObjectQuery<TEntity> query)
          where TEntity : class;


        Task<int> DeleteAsync<TEntity>(
            TDbContext dbContext, 
            EntityMap entityMap, 
            ObjectQuery<TEntity> query)
           where TEntity : class;
    }
}
