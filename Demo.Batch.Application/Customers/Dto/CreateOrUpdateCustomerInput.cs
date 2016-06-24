using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Demo.Batch.Customers;

namespace Demo.Batch.Application.Customers.Dto
{
    [AutoMap(typeof(Customer))]
    public class CreateOrUpdateCustomerInput : EntityDto<long?>, IInputDto
    {
        [StringLength(Customer.NameMaxLength)]
        public string FirstName { get; set; }

        [StringLength(Customer.NameMaxLength)]
        public string LastName { get; set; }

        public int Age { get; set; }
    }
}
