using System.Globalization;

namespace Abp.Runtime.DataAnnotations
{
    public interface IDataAnnotationLocalizable
    {
        string GetString(string name);
        string GetString(string name, CultureInfo culture);
    }
}
