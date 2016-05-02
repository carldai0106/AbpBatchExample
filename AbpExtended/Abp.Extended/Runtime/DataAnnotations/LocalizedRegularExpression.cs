using System.ComponentModel.DataAnnotations;

namespace Abp.Runtime.DataAnnotations
{
    public class LocalizedRegularExpression : RegularExpressionAttribute
    {
        public LocalizedRegularExpression(string pattern)
            : base(pattern)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(LocalizedHelper.L(ErrorMessage), name);
        }       
    }
}
