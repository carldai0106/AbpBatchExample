using System.Data.Common;

namespace Demo.Batch.Configuration
{
    public interface IBatchDataModuleConfiguration
    {
        string NameOrConnectionString { get; set; }
        DbConnection Connection { get; set; }
    }
}
