using System.Reflection;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Json;
using Abp.Modules;
using Abp.Runtime.DataAnnotations;

namespace Abp
{
    public class AbpExtendedModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.RegisterIfNot<IAbpExtendedConfiguration, AbpExtendedConfiguration>();
            IocManager.RegisterIfNot<IDataAnnotationLocalizable, DataAnnotationLocalization>();

            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    AbpExtendedConsts.LocalizationSourceName,
                    new JsonEmbeddedFileLocalizationDictionaryProvider(
                        Assembly.GetExecutingAssembly(),
                        "Abp.Localization.Sources.JsonAbpExtended"
                        )
                    )
                );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}