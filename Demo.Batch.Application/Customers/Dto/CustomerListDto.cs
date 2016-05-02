using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Demo.Batch.Customers;

namespace Demo.Batch.Application.Customers.Dto
{
    [AutoMap(typeof(Customer))]
    public class CustomerListDto : AuditedEntityDto<long>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Age { get; set; }
    }
}
