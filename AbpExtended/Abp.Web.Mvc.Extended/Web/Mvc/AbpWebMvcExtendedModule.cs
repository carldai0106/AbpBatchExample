using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Json;
using Abp.Modules;
using Abp.Runtime.DataAnnotations;
using Abp.Web.Mvc.DataAnnotations;

namespace Abp.Web.Mvc
{
    public class AbpWebMvcExtendedModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Localization.Sources.Add(
               new DictionaryBasedLocalizationSource(
                   AbpMvcExtendedConsts.LocalizationSourceName,
                   new JsonEmbeddedFileLocalizationDictionaryProvider(
                       Assembly.GetExecutingAssembly(),
                       "Abp.Web.Mvc.Resources.Json"
                       )
                   )
               );

            //找出缺省的客户端数据验证类型
            var clientDataTypeValidator = ModelValidatorProviders.Providers.OfType<ClientDataTypeModelValidatorProvider>().FirstOrDefault();
            if (null != clientDataTypeValidator)
            {
                //如果有匹配删除该类型
                ModelValidatorProviders.Providers.Remove(clientDataTypeValidator);
            }

            //添加自定义的验证类型
            ModelValidatorProviders.Providers.Add(new FilterableClientDataTypeModelValidatorProvider());

            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(LocalizedRequired), typeof(RequiredAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(LocalizedStringLength), typeof(StringLengthAttributeAdapter));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(LocalizedRegularExpression), typeof(RegularExpressionAttributeAdapter));
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
