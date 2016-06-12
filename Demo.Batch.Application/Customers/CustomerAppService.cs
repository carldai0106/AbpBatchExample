using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Demo.Batch.Application.Customers.Dto;
using Demo.Batch.Customers;

namespace Demo.Batch.Application.Customers
{
    public class CustomerAppService : BatchAppServiceBase, ICustomerAppService
    {
        private readonly IBatchRepository<Customer, long> _repository;
        private CustomerManager _mananger;
        public CustomerAppService(
            CustomerManager mananger,
            IBatchRepository<Customer, long> repository)
        {
            _mananger = mananger;
            _repository = repository;
        }

        public async Task<ListResultOutput<CustomerListDto>> GetCustomers()
        {
            var list = await _repository.GetAllListAsync();

            await _mananger.GetCustomers();

            return new ListResultOutput<CustomerListDto>(list.MapTo<List<CustomerListDto>>());
        }

        public async Task CreateOrUpdateCustomer()
        {
            var list = new List<Customer>
            {
                new Customer
                {
                    FirstName = "Carl",
                    LastName = "Dai",
                    TenantId = 1,
                    Age = 31
                },
                new Customer
                {
                    FirstName = "Perry",
                    LastName = "Yu",
                    TenantId = 1,
                    Age = 28
                }
            };

            await _mananger.CreateOrUpdateCustomer(list);

            await _repository.BatchInsertAsync(list);
        }

        public async Task BatchDelete()
        {
            var list = new List<long> {1, 2, 3, 4, 5, 6};

            await _mananger.BatchDelete(list);

            await _repository.BatchDeleteAsync(x => list.Contains(x.Id));

            await _repository.BatchDeleteAsync(x => x.Age >= 55 && x.TenantId == 1 && x.IsDeleted == false);
        }

        public async Task BatchUpdate()
        {
            await _mananger.BatchUpdate();

            await _repository.BatchUpdateAsync(x => x.Age < 30, x2 => new Customer {
                Age = x2.Age + 10
            });
        }
    }
}
