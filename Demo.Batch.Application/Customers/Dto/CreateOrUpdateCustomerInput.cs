using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Demo.Batch.Customers;

namespace Demo.Batch.Application.Customers.Dto
{
    public class CreateOrUpdateCustomerInput : EntityDto<long?>, IInputDto
    {
        [StringLength(Customer.NameMaxLength)]
        public string FirstName { get; set; }

        [StringLength(Customer.NameMaxLength)]
        public string LastName { get; set; }

        public int Age { get; set; }
    }
}
