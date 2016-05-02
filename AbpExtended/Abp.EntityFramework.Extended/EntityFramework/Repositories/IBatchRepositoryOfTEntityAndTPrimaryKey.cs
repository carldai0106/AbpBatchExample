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
        Task BatchInsertAsync(IEnumerable<TEntity> entities);       

        Task BatchUpdateAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> updateExpression);

        Task BatchDeleteAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
