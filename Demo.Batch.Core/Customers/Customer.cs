using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Demo.Batch.Customers
{
    [Table("Customer")]
    public class Customer : FullAuditedAndTenantEntity<long>
        //FullAuditedEntity<long>
        //Entity<long>,
        //IMayHaveTenant
    {
        public const int NameMaxLength = 16;

        [StringLength(NameMaxLength)]
        public string FirstName { get; set; }

        [StringLength(NameMaxLength)]
        public string LastName { get; set; }

        public int Age { get; set; }
        //public int? TenantId { get; set; }
    }
}
