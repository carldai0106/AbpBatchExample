using System.Reflection;
using Abp;
using Abp.Dependency;
using Abp.EntityFramework;
using Abp.Modules;
using Demo.Batch.Configuration;

namespace Demo.Batch
{
    [DependsOn(
        typeof(BatchCoreModule),        
        typeof(AbpEntityFrameworkExModule)
        )]
    public class BatchDataModule : AbpModule
    {
        public override void PreInitialize()
        {
            //web.config (or app.config for non-web projects) file should containt a connection string named "Default".
            if (!DebugHelper.IsDebug)
            {
                //Configuration.DefaultNameOrConnectionString = "BcsVhas";
                IocManager.RegisterIfNot<IBatchDataModuleConfiguration, BatchDataModuleConfiguration>();
            }
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
