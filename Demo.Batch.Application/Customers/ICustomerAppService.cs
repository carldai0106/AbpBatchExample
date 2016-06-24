using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Demo.Batch.Application.Customers.Dto;

namespace Demo.Batch.Application.Customers
{
    public interface ICustomerAppService : IApplicationService
    {
        Task<ListResultOutput<CustomerListDto>> GetCustomers();

        Task BatchCreate(IEnumerable<CreateOrUpdateCustomerInput> input);

        Task<ListResultOutput<CustomerListDto>> GetCustomerByName(string firstName, string lastName);

        Task BatchDelete(IEnumerable<long> list);

        Task<ListResultOutput<CustomerListDto>> GetCustomerByAge(int assignedAge);

        Task<int> BatchUpdateForCustomerAge(int assignedAge);
    }
}
