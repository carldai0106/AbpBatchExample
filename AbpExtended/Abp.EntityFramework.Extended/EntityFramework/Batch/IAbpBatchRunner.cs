using System;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EntityFramework.Mapping;
using System.Data.Entity.Core.Objects;

namespace Abp.EntityFramework.Batch
{
    public interface IAbpBatchRunner<TDbContext>  where TDbContext : DbContext
    {
        Task<int> UpdateAsync<TEntity>(
            TDbContext dbtContext, 
            EntityMap entityMap, 
            ObjectQuery<TEntity> query, 
            Expression<Func<TEntity, TEntity>> updateExpression)
            where TEntity : class;

        Task<int> DeleteAsync<TEntity>(
            TDbContext dbContext, 
            EntityMap entityMap, 
            ObjectQuery<TEntity> query)
           where TEntity : class;
    }
}
