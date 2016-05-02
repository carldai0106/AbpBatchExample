using System.Reflection;
using Abp.Modules;
using Demo.Batch.Application;

namespace Demo.Batch.Tests
{
    [DependsOn(
        typeof(BatchCoreModule),
        typeof(BatchApplicationModule))]
    public class BatchTestModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
