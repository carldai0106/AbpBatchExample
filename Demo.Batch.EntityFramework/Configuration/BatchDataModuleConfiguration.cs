using System.Data.Common;

namespace Demo.Batch.Configuration
{
    public class BatchDataModuleConfiguration : IBatchDataModuleConfiguration
    {
        public string NameOrConnectionString { get; set; }
        public DbConnection Connection { get; set; }
    }
}
