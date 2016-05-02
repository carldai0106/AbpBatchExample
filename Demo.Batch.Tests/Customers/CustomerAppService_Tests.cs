using System.Linq;
using System.Threading.Tasks;
using Demo.Batch.Application.Customers;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Demo.Batch.Tests.Customers
{
    public class CustomerAppService_Tests : BatchTestBase
    {
        private readonly ICustomerAppService _service;

        public CustomerAppService_Tests(ITestOutputHelper output) : 
            base(output)
        {
            _service = Resolve<ICustomerAppService>();
        }

        [Fact]
        public async Task GetCustomers_Test()
        {
            var list = await _service.GetCustomers();

            var json = JsonConvert.SerializeObject(list, Formatting.Indented);

            Output.WriteLine(json);
        }

        [Fact]
        public async Task CreateCustomers_Test()
        {
            await _service.CreateOrUpdateCustomer();

            UsingDbContext(c =>
            {
                c.Customers.FirstOrDefault(x => x.FirstName == "Carl").ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task BatchDelete_Test()
        {
            await _service.BatchDelete();
        }

        [Fact]
        public async Task BatchUpdate_Test()
        {
            await _service.BatchUpdate();
        }
    }
}
