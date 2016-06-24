using Abp;
using Demo.Batch.EntityFramework;
using Demo.Batch.Migrations.Seed;

namespace Demo.Batch.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<Demo.Batch.EntityFramework.BatchDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;

            ContextKey = "Batch";
        }

        protected override void Seed(BatchDbContext context)
        {
            new BatchDbInit(context).Create();
        }
    }
}
