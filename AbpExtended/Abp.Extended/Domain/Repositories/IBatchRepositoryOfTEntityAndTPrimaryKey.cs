using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace Abp.Domain.Repositories
{
    public interface IBatchRepository<TEntity, TPrimaryKey> :
        IRepository<TEntity, TPrimaryKey> where TEntity : 
        class, IEntity<TPrimaryKey>
    {
        /// <summary>
        /// Batch insert, Notice : You can use Effort.EF6 to test
        /// </summary>
        /// <param name="entities">entities</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> BatchInsertAsync(IEnumerable<TEntity> entities);       

        /// <summary>
        /// Batch update, Notice : You can not use Effort.EF6 to test, you must use a real database.
        /// </summary>
        /// <param name="predicate">predicate</param>
        /// <param name="updateExpression">update expression</param>
        /// <returns></returns>
        Task<int> BatchUpdateAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> updateExpression);

        /// <summary>
        /// Batch delete, Notice : You can not use Effort.EF6 to test, you must use a real database.
        /// </summary>
        /// <param name="predicate">predicate</param>
        /// <returns></returns>
        Task<int> BatchDeleteAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
