using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using System.Linq;
using Demo.Batch.Application.Customers.Dto;
using Demo.Batch.Customers;

namespace Demo.Batch.Application.Customers
{
    public class CustomerAppService : BatchAppServiceBase, ICustomerAppService
    {
        private readonly IBatchRepository<Customer, long> _repository;
        public CustomerAppService(
            IBatchRepository<Customer, long> repository)
        {
            _repository = repository;
        }

        public async Task<ListResultOutput<CustomerListDto>> GetCustomers()
        {
            var list = await _repository.GetAllListAsync();
            return new ListResultOutput<CustomerListDto>(list.MapTo<List<CustomerListDto>>());
        }

        public async Task BatchCreate(IEnumerable<CreateOrUpdateCustomerInput> input)
        {
            var list = input.Select(x =>
            {
                var entity = new Customer
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age

                };

                var prop = typeof(Customer).GetProperty("TenantId");
                prop?.SetValue(entity, 1);

                return entity;
            });

            await _repository.BatchInsertAsync(list);
        }

        public async Task<ListResultOutput<CustomerListDto>> GetCustomerByName(string firstName, string lastName)
        {
            var list = await _repository.GetAllListAsync(x => x.FirstName == firstName && x.LastName == lastName);

            return new ListResultOutput<CustomerListDto>(list.MapTo<List<CustomerListDto>>());
        }

        public async Task BatchDelete(IEnumerable<long> input)
        {
            await _repository.BatchDeleteAsync(x => input.Contains(x.Id));
        }

        public async Task<ListResultOutput<CustomerListDto>> GetCustomerByAge(int assignedAge)
        {
            var list = await _repository.GetAllListAsync(x => x.Age < assignedAge);
            return new ListResultOutput<CustomerListDto>(list.MapTo<List<CustomerListDto>>());
        }

        public async Task<int> BatchUpdateForCustomerAge(int assignedAge)
        {
            return await _repository.BatchUpdateAsync(x => x.Age < assignedAge, x2 => new Customer
            {
                Age = x2.Age + (assignedAge - x2.Age)
            });
        }
       
    }
}
