using System;
using System.Globalization;
using Abp.Dependency;

namespace Abp.Runtime.DataAnnotations
{
    internal class LocalizedHelper
    {
        private static readonly Lazy<IDataAnnotationLocalizable> DataAnnotationLocalizable;

        static LocalizedHelper()
        {
            DataAnnotationLocalizable = new Lazy<IDataAnnotationLocalizable>(
                () => IocManager.Instance.Resolve<IDataAnnotationLocalizable>()
                );
        }

        public static string L(string name)
        {
            return DataAnnotationLocalizable.Value.GetString(name);
        }

        public static string L(string name, CultureInfo culture)
        {
            return DataAnnotationLocalizable.Value.GetString(name, culture);
        }
    }
}
