using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Demo.Batch.Application.Customers.Dto;

namespace Demo.Batch.Application.Customers
{
    public interface ICustomerAppService : IApplicationService
    {
        Task<ListResultOutput<CustomerListDto>> GetCustomers();

        Task CreateOrUpdateCustomer();

        Task BatchDelete();

        Task BatchUpdate();
    }
}
