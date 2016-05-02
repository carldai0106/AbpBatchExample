namespace Abp.Configuration.Startup
{
    public static class AbpExtendedConfigurationExtensions
    {
        public static IAbpExtendedConfiguration AbpExtended(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.GetOrCreate("Modules.Abp.Extended", () => configurations.AbpConfiguration.IocManager.Resolve<IAbpExtendedConfiguration>());
        }
    }
}
