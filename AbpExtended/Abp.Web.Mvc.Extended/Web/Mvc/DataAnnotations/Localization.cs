using System.Globalization;
using Abp.Dependency;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.Web.Mvc.Configuration;

namespace Abp.Web.Mvc.Localization
{
    internal class Localization : ITransientDependency
    {
        private readonly ILocalizationSource LocalizationSource;
        public Localization(IAbpMvcExtendedConfiguration configuration)
        {
            LocalizationSource = LocalizationHelper.GetSource(configuration.LocalizationSourceName);
        }

        public string GetString(string name)
        {
            return LocalizationSource.GetString(name);
        }

        public string GetString(string name, CultureInfo culture)
        {
            return LocalizationSource.GetString(name, culture);
        }

        public string GetString(string localizationSourceName, string name)
        {
            return LocalizationHelper.GetSource(localizationSourceName).GetString(name);
        }

        public string GetString(string localizationSourceName, string name, CultureInfo culture)
        {
            return LocalizationHelper.GetSource(localizationSourceName).GetString(name, culture);
        }
    }
}
