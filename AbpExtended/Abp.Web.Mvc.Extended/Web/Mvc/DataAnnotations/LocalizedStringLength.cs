using System.ComponentModel.DataAnnotations;

namespace Abp.Web.Mvc.Localization
{
    public class LocalizedStringLength : StringLengthAttribute
    {
        public LocalizedStringLength(int maximumLength)
            : base(maximumLength)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            
            return string.Format(TranslationHelper.GetString(ErrorMessage), name, MinimumLength, MaximumLength);
        }
    }
}
