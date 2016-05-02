using System.ComponentModel;

namespace Abp.Web.Mvc.Localization
{
    public class LocalizedDescription : DescriptionAttribute
    {
        private readonly string _description;
        public LocalizedDescription(string description)
        {
            _description = TranslationHelper.GetString(description);
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
