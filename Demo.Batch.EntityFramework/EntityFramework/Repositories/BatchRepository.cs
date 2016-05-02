using Abp.Domain.Entities;
using Abp.EntityFramework;

namespace Demo.Batch.EntityFramework.Repositories
{
    public class BatchRepository<TEntity, TPrimaryKey> :
       BatchRepositoryBase<BatchDbContext, TEntity, TPrimaryKey>
       where TEntity : class, IEntity<TPrimaryKey>
    {
        public BatchRepository(IDbContextProvider<BatchDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }
    }
}
