using System;
using System.Threading.Tasks;
using Abp.Collections;
using Abp.Modules;
using Abp.TestBase;
using Demo.Batch.Configuration;
using Demo.Batch.EntityFramework;
using Demo.Batch.Migrations.Seed;
using Effort;
using EntityFramework.DynamicFilters;
using Xunit.Abstractions;

namespace Demo.Batch.Tests
{
    public class BatchTestBase : AbpIntegratedTestBase
    {
        protected readonly ITestOutputHelper Output;
        protected BatchTestBase(ITestOutputHelper output)
        {
            UsingDbContext(context =>
            {
                new BatchDbInit(context).Create();
            });

            Output = output;

            LoginAsTenant();
        }

        public void LoginAsTenant()
        {
            AbpSession.TenantId = 1;
            AbpSession.UserId = 1;
        }

        protected override void PreInitialize()
        {
            var authConnection = DbConnectionFactory.CreateTransient();
            if (!LocalIocManager.IsRegistered<IBatchDataModuleConfiguration>())
            {
                LocalIocManager.Register<IBatchDataModuleConfiguration, BatchDataModuleConfiguration>();
                Resolve<IBatchDataModuleConfiguration>().Connection = authConnection;
            }
        }

        protected override void AddModules(ITypeList<AbpModule> modules)
        {
            base.AddModules(modules);
            //Adding testing modules. Depended modules of these modules are automatically added.

            modules.Add<BatchTestModule>();
        }

        protected void UsingDbContext(Action<BatchDbContext> action)
        {
            using (var context = LocalIocManager.Resolve<BatchDbContext>())
            {
                context.DisableAllFilters();
                action(context);
                context.SaveChanges();
            }
        }

        protected async Task UsingDbContextAsync(Action<BatchDbContext> action)
        {
            using (var context = LocalIocManager.Resolve<BatchDbContext>())
            {
                context.DisableAllFilters();
                action(context);
                await context.SaveChangesAsync();
            }
        }

        protected T UsingDbContext<T>(Func<BatchDbContext, T> func)
        {
            T result;

            using (var context = LocalIocManager.Resolve<BatchDbContext>())
            {
                context.DisableAllFilters();
                result = func(context);
                context.SaveChanges();
            }

            return result;
        }

        protected async Task<T> UsingDbContextAsync<T>(Func<BatchDbContext, Task<T>> func)
        {
            T result;

            using (var context = LocalIocManager.Resolve<BatchDbContext>())
            {
                context.DisableAllFilters();
                result = await func(context);
                await context.SaveChangesAsync();
            }

            return result;
        }
    }
}
