using System.ComponentModel.DataAnnotations;

namespace Abp.Runtime.DataAnnotations
{
    public class LocalizedRequired : RequiredAttribute
    {
        public override string FormatErrorMessage(string name)
        {
            return string.Format(LocalizedHelper.L(ErrorMessageString), name);
        }
    }
}
