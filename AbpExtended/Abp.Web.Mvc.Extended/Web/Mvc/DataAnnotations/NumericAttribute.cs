using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Abp.Localization;

namespace Abp.Web.Mvc.DataAnnotations
{
    /// <summary>
    /// 创建一个密封的数字验证特性类
    /// </summary>
    internal class NumericAttribute : ValidationAttribute, IClientValidatable
    {
        public override bool IsValid(object value)
        {
            return true;
        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ValidationType = "number", 
                ErrorMessage = FormatErrorMessage(metadata.DisplayName)
            };
        }
    }

    /// <summary>
    /// 创建用于客户端验证的验证提供器
    /// </summary>
    public class FilterableClientDataTypeModelValidatorProvider : ClientDataTypeModelValidatorProvider
    {
        public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context)
        {
            var allValidators = base.GetValidators(metadata, context);
            var validators = new List<ModelValidator>();
            foreach (var v in allValidators)
            {
                //如果不是系统默认的数字验证类，则用系统的
                if (v.GetType().Name != "NumericModelValidator")
                {
                    validators.Add(v);
                }
                else
                {
                    NumericAttribute attribute;
                    var lang = LocalizationHelper.GetString(AbpMvcExtendedConsts.LocalizationSourceName, "NumericModelValidator");

                    if (lang != "[NumericModelValidator]")
                    {
                        //用自定义替换系统的数字验证
                        attribute = new NumericAttribute {ErrorMessage = lang};
                    }
                    else
                    {
                        attribute = new NumericAttribute();
                    }

                    var validator = new DataAnnotationsModelValidator(metadata, context, attribute);
                    validators.Add(validator);
                }
            }
            return validators;
        }
    }
}
