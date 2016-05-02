using System;
using System.Data.Entity;
using System.Linq;
using Demo.Batch.EntityFramework;

namespace Demo.Batch.Migrations.Seed
{
    public class DefaultBaseCreator<TEntity> where TEntity : class
    {
        protected BatchDbContext BatchContext
        {
            get;
            private set;
        }

        protected int? TenantId { get; set; }

        public DefaultBaseCreator(BatchDbContext context)
        {
            BatchContext = context;

            TenantId = 1;
        }

        protected void IsNullAndThrowException<T>(T t) where T : class
        {
            if (t == null)
            {
                throw new Exception("Please init data for type : " + typeof(T).Name);
            }
        }

        public IQueryable<TEntity> GetAll()
        {
            return Table;
        }

        protected virtual void AttachIfNot(TEntity entity)
        {
            if (!Table.Local.Contains(entity))
            {
                Table.Attach(entity);
            }
        }

        public virtual DbSet<TEntity> Table
        {
            get
            {
                return BatchContext.Set<TEntity>();
            }
        }

        public TEntity Insert(TEntity entity)
        {
            var model = Table.Add(entity);
            return model;
        }
    }
}
