using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Abp;
using Abp.EntityFramework;
using Demo.Batch.Configuration;
using Demo.Batch.Customers;

namespace Demo.Batch.EntityFramework
{
    public class BatchDbContext : AbpDbContext
    {
        public virtual IDbSet<Customer> Customers { get; set; }

        static BatchDbContext()
        {
            if (!DebugHelper.IsDebug)
            {
                Database.SetInitializer(new CreateDatabaseIfNotExists<BatchDbContext>());
                Database.SetInitializer(new MigrateDatabaseToLatestVersion<BatchDbContext, Migrations.Configuration>());
            }
        }

        /* Setting "Default" to base class helps us when working migration commands on Package Manager Console.
        * But it may cause problems when working Migrate.exe of EF. ABP works either way.         * 
        */
        public BatchDbContext()
            : base("DemoBatch")
        {
            //Update-Database -ConnectionString "Server=CARL\MSSQL2012; Database=DemoBatch; Trusted_Connection=Yes;" -ConnectionProviderName "System.Data.SqlClient" -Verbose -ProjectName "Demo.Batch.EntityFramework"
        }

        /* This constructor is used by ABP to pass connection string defined in AbpZeroTemplateDataModule.PreInitialize.
         * Notice that, actually you will not directly create an instance of AbpZeroTemplateDbContext since ABP automatically handles it.
         */
        public BatchDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        public BatchDbContext(IBatchDataModuleConfiguration configuration)
#if DEBUG
            : base(configuration.Connection, true)
#else
            : base(configuration.NameOrConnectionString)
#endif
        {

        }

        /* This constructor is used in tests to pass a fake/mock connection.
         */
        public BatchDbContext(DbConnection dbConnection)
            : base(dbConnection, true)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
        }
    }
}
