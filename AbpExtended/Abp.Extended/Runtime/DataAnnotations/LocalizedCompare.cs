using System.ComponentModel.DataAnnotations;

namespace Abp.Runtime.DataAnnotations
{
    public class LocalizedCompare : CompareAttribute
    {
        public LocalizedCompare(string otherProperty)
            : base(otherProperty)
        {
        }

		public override string FormatErrorMessage(string name)
		{
            return string.Format(LocalizedHelper.L(ErrorMessageString), OtherPropertyDisplayName, name);
		}
    }
}
