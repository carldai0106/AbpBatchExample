using System.Globalization;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Localization;
using Abp.Localization.Sources;

namespace Abp.Runtime.DataAnnotations
{
    internal class DataAnnotationLocalization : IDataAnnotationLocalizable, ISingletonDependency
    {
        public IAbpExtendedConfiguration AbpExtendedConfiguration { get; set; }

        public DataAnnotationLocalization()
        {

        }

        public string GetString(string name)
        {
            return GetLocalizationSource().GetString(name);
        }

        public string GetString(string name, CultureInfo culture)
        {
            return GetLocalizationSource().GetString(name, culture);
        }

        private ILocalizationSource GetLocalizationSource()
        {
            return LocalizationHelper.GetSource(AbpExtendedConfiguration.LocalizationSourceName);
        }
    }
}
