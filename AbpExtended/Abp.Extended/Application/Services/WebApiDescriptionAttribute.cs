using System;

namespace Abp.Application.Services
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface)]
    public class WebApiDescriptionAttribute : Attribute
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public WebApiDescriptionAttribute(string description)
        {
            Description = description;
        }

        public WebApiDescriptionAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
