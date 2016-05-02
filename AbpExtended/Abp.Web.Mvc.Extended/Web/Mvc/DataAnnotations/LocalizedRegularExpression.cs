using System.ComponentModel.DataAnnotations;

namespace Abp.Web.Mvc.Localization
{
    public class LocalizedRegularExpression : RegularExpressionAttribute
    {
        public LocalizedRegularExpression(string pattern)
            : base(pattern)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(TranslationHelper.GetString(ErrorMessage), name);
        }       
    }
}
