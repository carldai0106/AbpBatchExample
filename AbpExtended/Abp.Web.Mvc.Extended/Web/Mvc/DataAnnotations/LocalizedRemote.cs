using System.Web.Mvc;
using Abp.Runtime.DataAnnotations;

namespace Abp.Web.Mvc.DataAnnotations
{
    public class LocalizedRemote : RemoteAttribute
    {
        public LocalizedRemote(string routeName)
            : base(routeName)
        {
        }

        public LocalizedRemote(string action, string controller)
            : base(action, controller)
        {
        }

        public LocalizedRemote(string action, string controller, string areaName)
            : base(action, controller, areaName)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(LocalizedHelper.L(ErrorMessage), name);
        }
    }
}
