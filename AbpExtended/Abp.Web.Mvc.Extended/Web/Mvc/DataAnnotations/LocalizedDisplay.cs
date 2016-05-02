using System.ComponentModel;

namespace Abp.Web.Mvc.Localization
{
    public class LocalizedDisplay : DisplayNameAttribute
    {
        private readonly string _nameOrKey;
        public LocalizedDisplay(string nameOrKey)
        {
            _nameOrKey = nameOrKey;
        }

        public override string DisplayName
        {
            get
            {
                return TranslationHelper.GetString(_nameOrKey);
            }
        }
    }
}
