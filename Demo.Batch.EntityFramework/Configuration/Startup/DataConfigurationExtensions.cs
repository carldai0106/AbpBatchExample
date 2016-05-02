using Abp.Configuration.Startup;

namespace Demo.Batch.Configuration.Startup
{
    public static class BatchDataConfigurationExtensions
    {
        public static IBatchDataModuleConfiguration BatchDataModule(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.GetOrCreate("Modules.Demo.Batch", () => configurations.AbpConfiguration.IocManager.Resolve<IBatchDataModuleConfiguration>());
        }
    }
}
