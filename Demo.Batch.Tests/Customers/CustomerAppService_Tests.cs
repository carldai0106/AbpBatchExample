using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Batch.Application.Customers;
using Demo.Batch.Application.Customers.Dto;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using System.Linq;

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

        [Fact, TestPriority(1)]
        public async Task GetCustomers_Test()
        {
            var list = await _service.GetCustomers();

            var json = JsonConvert.SerializeObject(list, Formatting.Indented);

            Output.WriteLine(json);
        }

        [Fact, TestPriority(2)]
        public async Task BatchCreate_Test()
        {
            var rs = await _service.GetCustomers();
            var count = rs.Items.Count;

            var list = new List<CreateOrUpdateCustomerInput>
            {
                new CreateOrUpdateCustomerInput
                {
                    FirstName = "Carl",
                    LastName = "Dai",
                    Age = 31
                },
                new CreateOrUpdateCustomerInput
                {
                    FirstName = "Perry",
                    LastName = "Yu",
                    Age = 31
                }
            };

            await _service.BatchCreate(list);
            rs = await _service.GetCustomers();
            rs.Items.Count.ShouldBe(count + list.Count);
        }

        //Notice : Run this method before you must run Batch Insert method.
        [Fact, TestPriority(3)]
        public async Task BatchDelete_Test()
        {
            var rsItem1 = await _service.GetCustomerByName("Carl", "Dai");
            var rsItem2 = await _service.GetCustomerByName("Perry", "Yu");

            var list = new List<long>
            {
                rsItem1.Items.ElementAt(0).Id,
                rsItem2.Items.ElementAt(0).Id
            };

            await _service.BatchDelete(list);

            var rs = await _service.GetCustomers();
            var count = rs.Items.Count(x => list.Contains(x.Id));

            count.ShouldBe(0);
        }
        
        [Fact, TestPriority(5)]
        public async Task BatchDelete_Random_ID_Test()
        {
            var rs = await _service.GetCustomers();
            var ids = rs.Items.Select(x => x.Id).OrderBy(x => Abp.RandomHelper.GetRandom(1, 100));
            var list = ids.Take(20);
            await _service.BatchDelete(list);
        }

        [Fact, TestPriority(4)]
        public async Task BatchUpdate_Test()
        {
            var rs = await _service.GetCustomerByAge(30);
            var count = rs.Items.Count;

            await _service.BatchUpdateForCustomerAge(30);

            rs = await _service.GetCustomerByAge(30);

            rs.Items.Count.ShouldNotBe(count);

            await _service.BatchUpdateForCustomerAge(20);
        }
    }
}
