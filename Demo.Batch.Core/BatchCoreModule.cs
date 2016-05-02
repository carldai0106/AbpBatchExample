using System.Reflection;
using Abp;
using Abp.Modules;

namespace Demo.Batch
{
    [DependsOn(typeof(AbpExtendedModule))]
    public class BatchCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.MultiTenancy.IsEnabled = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
