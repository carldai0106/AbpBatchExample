using System.Reflection;
using Abp.Collections.Extensions;
using Abp.Modules;
using Abp.Dependency;
using Abp.EntityFramework.Batch;
using Abp.Reflection;
using System;

namespace Abp.EntityFramework
{
    [DependsOn(
        typeof(AbpEntityFrameworkModule))]
    public class AbpEntityFrameworkExModule : AbpModule
    {
        private readonly ITypeFinder _typeFinder;
        public AbpEntityFrameworkExModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        //public override void PostInitialize()
        //{
        //    RegisterGenericRepositories();
        //}

        private void RegisterGenericRepositories()
        {
            var dbContextTypes =
                _typeFinder.Find(type =>
                    type.IsPublic &&
                    !type.IsAbstract &&
                    type.IsClass &&
                    typeof(AbpDbContext).IsAssignableFrom(type)
                    );

            if (dbContextTypes.IsNullOrEmpty())
            {
                return;
            }

            for (var i = 0; i < dbContextTypes.Length; i++)
            {
                var item = dbContextTypes[i];

                var type = typeof(IAbpBatchRunner<>).MakeGenericType(item);
                var implType = typeof(AbpSqlServerBatchRunner<>).MakeGenericType(item);
                IocManager.RegisterIfNot(type, implType, DependencyLifeStyle.Transient);
            }            
        }
    }
}
