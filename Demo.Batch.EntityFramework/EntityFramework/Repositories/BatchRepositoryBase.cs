using System.Data.Entity;
using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;

namespace Demo.Batch.EntityFramework.Repositories
{
    public abstract class BatchRepositoryBase<TDbContext, TEntity, TPrimaryKey> :
        BatchEfRepositoryBase<TDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
        where TDbContext : DbContext
    {
        protected BatchRepositoryBase(IDbContextProvider<TDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }
    }
}
