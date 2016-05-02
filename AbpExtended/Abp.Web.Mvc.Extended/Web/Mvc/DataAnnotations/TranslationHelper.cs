using System;
using System.Globalization;
using Abp.Dependency;

namespace Abp.Web.Mvc.Localization
{
    internal class TranslationHelper
    {
        //private static readonly Lazy<Localization> Localization;

        //static TranslationHelper()
        //{
        //    Localization = new Lazy<Localization>(() => IocManager.Instance.Resolve<Localization>());
        //}

        //private static Localization GetInstance()
        //{
        //    return IocManager.Instance.Resolve<Localization>();
        //}

        private static Localization Localization
        {
            get
            {
                return IocManager.Instance.Resolve<Localization>();
            }
        }

        public static string GetString(string name)
        {
            return Localization.GetString(name);
        }

        public static string GetString(string name, CultureInfo culture)
        {
            return Localization.GetString(name, culture);
        }

        public static string GetString(string localizationSourceName, string name)
        {
            return Localization.GetString(localizationSourceName, name);
        }

        public static string GetString(string localizationSourceName, string name, CultureInfo culture)
        {
            return Localization.GetString(localizationSourceName, name, culture);
        }
    }
}
