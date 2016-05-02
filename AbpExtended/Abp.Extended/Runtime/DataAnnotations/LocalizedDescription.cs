using System.ComponentModel;

namespace Abp.Runtime.DataAnnotations
{
    public class LocalizedDescription : DescriptionAttribute
    {
        private readonly string _description;
        public LocalizedDescription(string description)
        {
            _description = LocalizedHelper.L(description);
            DescriptionValue = _description;
        }

        public override string Description
        {
            get
            {
                return _description;
            }
        }
    }
}
