using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;

namespace Demo.Batch.Customers
{
    public class CustomerManager : BatchDomainService
    {
        private readonly IBatchRepository<Customer, long> _repository;
        public CustomerManager(IBatchRepository<Customer, long> repository)
        {
            _repository = repository;
        }

        public async Task<List<Customer>> GetCustomers()
        {
            var list = await _repository.GetAllListAsync();

            return list;
        }

        public async Task<IEnumerable<Customer>> CreateOrUpdateCustomer(IEnumerable<Customer> list)
        {
            return await _repository.BatchInsertAsync(list);
        }

        [UnitOfWork]
        public async Task BatchDelete(IEnumerable<long> list)
        {
            await _repository.BatchDeleteAsync(x => list.Contains(x.Id));
            await _repository.BatchDeleteAsync(x => x.Age >= 55 && x.TenantId == 1 && x.IsDeleted == false);
        }

        public async Task BatchUpdate()
        {
            await _repository.BatchUpdateAsync(x => x.Age < 30, x2 => new Customer
            {
                Age = x2.Age + 10
            });
        }
    }
}
