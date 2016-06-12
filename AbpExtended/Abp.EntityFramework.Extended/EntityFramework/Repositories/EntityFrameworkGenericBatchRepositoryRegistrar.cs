using System;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.EntityFramework.Extensions;
using Abp.Reflection.Extensions;

namespace Abp.EntityFramework.Repositories
{
    public class EntityFrameworkGenericBatchRepositoryRegistrar
    {
        public static void RegisterForDbContext(Type dbContextType, IIocManager iocManager)
        {
            var autoRepositoryAttr = dbContextType.GetSingleAttributeOrNull<AutoBatchRepositoryTypesAttribute>();
            if (autoRepositoryAttr == null)
            {
                autoRepositoryAttr = AutoBatchRepositoryTypesAttribute.Default;
            }

            foreach (var entityType in dbContextType.GetEntityTypes())
            {
                var primaryKeyType = EntityHelper.GetPrimaryKeyType(entityType);
                if (primaryKeyType == typeof(int))
                {
                    var genericRepositoryType = autoRepositoryAttr.RepositoryInterface.MakeGenericType(entityType);
                    if (!iocManager.IsRegistered(genericRepositoryType))
                    {
                        var implType = autoRepositoryAttr.RepositoryImplementation.GetGenericArguments().Length == 1
                                ? autoRepositoryAttr.RepositoryImplementation.MakeGenericType(entityType)
                                : autoRepositoryAttr.RepositoryImplementation.MakeGenericType(dbContextType, entityType);

                        iocManager.Register(
                            genericRepositoryType,
                            implType,
                            DependencyLifeStyle.Transient
                            );
                    }
                }

                var genericRepositoryTypeWithPrimaryKey = autoRepositoryAttr.RepositoryInterfaceWithPrimaryKey.MakeGenericType(entityType, primaryKeyType);
                if (!iocManager.IsRegistered(genericRepositoryTypeWithPrimaryKey))
                {
                    var implType = autoRepositoryAttr.RepositoryImplementationWithPrimaryKey.GetGenericArguments().Length == 2
                                ? autoRepositoryAttr.RepositoryImplementationWithPrimaryKey.MakeGenericType(entityType, primaryKeyType)
                                : autoRepositoryAttr.RepositoryImplementationWithPrimaryKey.MakeGenericType(dbContextType, entityType, primaryKeyType);

                    iocManager.Register(
                        genericRepositoryTypeWithPrimaryKey,
                        implType,
                        DependencyLifeStyle.Transient
                        );
                }
            }
        }
    }
}
