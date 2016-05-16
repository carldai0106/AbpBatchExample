using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;

namespace Abp.EntityFramework.Repositories
{
    public interface IBatchRepository<TEntity, TPrimaryKey> :
        IRepository<TEntity, TPrimaryKey> where TEntity : 
        class, IEntity<TPrimaryKey>
    {
        Task<IEnumerable<TEntity>> BatchInsertAsync(IEnumerable<TEntity> entities);       

        Task<int> BatchUpdateAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> updateExpression);

        Task<int> BatchDeleteAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
