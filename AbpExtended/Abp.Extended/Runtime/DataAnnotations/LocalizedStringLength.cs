using System.ComponentModel.DataAnnotations;

namespace Abp.Runtime.DataAnnotations
{
    public class LocalizedStringLength : StringLengthAttribute
    {
        public LocalizedStringLength(int maximumLength)
            : base(maximumLength)
        {
        }

        public override string FormatErrorMessage(string name)
        {

            return string.Format(LocalizedHelper.L(ErrorMessage), name, MinimumLength, MaximumLength);
        }
    }
}
