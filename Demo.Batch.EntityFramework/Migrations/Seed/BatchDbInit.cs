using Demo.Batch.EntityFramework;
using Demo.Batch.Migrations.Seed.Customers;
using EntityFramework.DynamicFilters;

namespace Demo.Batch.Migrations.Seed
{
    public class BatchDbInit
    {
        private readonly BatchDbContext _batchContext;

        public BatchDbInit(BatchDbContext batchContext)
        {
            _batchContext = batchContext;
        }

        public void Create()
        {
            _batchContext.DisableAllFilters();
            
            new DefaultCustomerCreator(_batchContext).Create();
            
            _batchContext.SaveChanges();
        }
    }
}
