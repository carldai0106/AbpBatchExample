using System.ComponentModel.DataAnnotations;

namespace Abp.Web.Mvc.Localization
{
    public class LocalizedCompare : CompareAttribute
    {
        public LocalizedCompare(string otherProperty)
            : base(otherProperty)
        {
        }

		public override string FormatErrorMessage(string name)
		{
            return string.Format(TranslationHelper.GetString(ErrorMessageString), OtherPropertyDisplayName, name);
		}
    }
}
