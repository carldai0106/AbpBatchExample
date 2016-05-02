using System.ComponentModel.DataAnnotations;

namespace Abp.Web.Mvc.Localization
{
    public class LocalizedRequired : RequiredAttribute
    {
        public override string FormatErrorMessage(string name)
        {
            return string.Format(TranslationHelper.GetString(ErrorMessageString), name);
        }
    }
}
