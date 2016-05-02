using System;

namespace Abp.Domain.Entities.Auditing
{
    [Serializable]
    public abstract class FullAuditedAndTenantEntity : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
    }
}