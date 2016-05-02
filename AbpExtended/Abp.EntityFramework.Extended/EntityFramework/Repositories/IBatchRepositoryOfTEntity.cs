
using Abp.Domain.Entities;

namespace Abp.EntityFramework.Repositories
{
    public interface IBatchRepository<TEntity> : 
        IBatchRepository<TEntity, int> where TEntity : 
        class, IEntity<int>
    {

    }
}
