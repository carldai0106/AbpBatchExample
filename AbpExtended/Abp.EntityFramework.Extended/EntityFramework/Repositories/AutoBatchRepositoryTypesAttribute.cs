using System;
using Abp.Domain.Repositories;

namespace Abp.EntityFramework.Repositories
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoBatchRepositoryTypesAttribute : Attribute
    {
        public static AutoBatchRepositoryTypesAttribute Default { get; private set; }

        public Type RepositoryInterface { get; private set; }

        public Type RepositoryInterfaceWithPrimaryKey { get; private set; }

        public Type RepositoryImplementation { get; private set; }

        public Type RepositoryImplementationWithPrimaryKey { get; private set; }

        static AutoBatchRepositoryTypesAttribute()
        {
            Default = new AutoBatchRepositoryTypesAttribute(
                typeof(IBatchRepository<>),
                typeof(IBatchRepository<,>),
                typeof(BatchEfRepositoryBase<,>),
                typeof(BatchEfRepositoryBase<,,>)
                );
        }

        public AutoBatchRepositoryTypesAttribute(
            Type repositoryInterface,
            Type repositoryInterfaceWithPrimaryKey,
            Type repositoryImplementation,
            Type repositoryImplementationWithPrimaryKey)
        {
            RepositoryInterface = repositoryInterface;
            RepositoryInterfaceWithPrimaryKey = repositoryInterfaceWithPrimaryKey;
            RepositoryImplementation = repositoryImplementation;
            RepositoryImplementationWithPrimaryKey = repositoryImplementationWithPrimaryKey;
        }
    }
}
