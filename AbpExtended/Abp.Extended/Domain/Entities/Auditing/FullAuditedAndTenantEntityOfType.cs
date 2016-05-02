using System;

namespace Abp.Domain.Entities.Auditing
{
    [Serializable]
    public abstract class FullAuditedAndTenantEntity<TPrimaryKey> :
        FullAuditedEntity<TPrimaryKey>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
    }
}