using System.Collections.Generic;
using System.Linq;
using Abp;
using Demo.Batch.Customers;
using Demo.Batch.EntityFramework;

namespace Demo.Batch.Migrations.Seed.Customers
{
    public class DefaultCustomerCreator : DefaultBaseCreator<Customer>
    {
        public DefaultCustomerCreator(BatchDbContext context) : base(context)
        {
        }

        public static string[] Names = new[]
        {
            "Agatha Christie",
            "Albert Einstein",
            "Aldous Huxley",
            "Amin Maalouf",
            "Andrew Andrewus",
            "Arda Turan",
            "Audrey Naulin",
            "Biff Tannen",
            "Bruce Wayne",
            "Butch Coolidge",
            "Carl Sagan",
            "Charles Quint",
            "Christophe Grange",
            "Christopher Nolan",
            "Christopher Lloyd",
            "Clara Clayton",
            "Clarice Starling",
            "Dan Brown",
            "Daniel Radcliffe",
            "Douglas Hall",
            "David Wells",
            "Emmett Brown",
            "Friedrich Hegel",
            "Forrest Gump",
            "Franz Kafka",
            "Gabriel Marquez",
            "Galileo Galilei",
            "Georghe Hagi",
            "Georghe Orwell",
            "Georghe Richards",
            "Gottfried Leibniz",
            "Hannibal Lecter",
            "Hercules Poirot",
            "Isaac Asimov",
            "Jane Fuller",
            "Jean Reno",
            "Jeniffer Parker",
            "Johan Elmander",
            "Jules Winnfield",
            "Kurt Vonnegut",
            "Laurence Fishburne",
            "Leo Tolstoy",
            "Lorraine Baines",
            "Marsellus Wallace",
            "Marty Mcfly",
            "Michael Corleone",
            "Oktay Anar",
            "Omer Hayyam",
            "Paulho Coelho",
            "Quentin Tarantino",
            "Rene Descartes",
            "Robert Lafore",
            "Stanislaw Lem",
            "Stefan Zweig",
            "Stephenie Mayer",
            "Stephen Hawking",
            "Thomas More",
            "Vincent Vega",
            "Vladimir Nabokov",
            "William Faulkner"
        };

        public List<T> GenerateRandomizedList<T>(IEnumerable<T> items)
        {
            var currentList = new List<T>(items);
            var randomList = new List<T>();

            while (currentList.Any())
            {
                var randomIndex = RandomHelper.GetRandom(0, currentList.Count);
                randomList.Add(currentList[randomIndex]);
                currentList.RemoveAt(randomIndex);
            }

            return randomList;
        }

        public List<Customer> GetRandomCustomers(int userCount, int tenantId)
        {
            var customers = new List<Customer>();

            var randomNames = GenerateRandomizedList(Names);
            for (var i = 0; i < userCount && i < randomNames.Count; i++)
            {
                customers.Add(CreateCustomer(tenantId, randomNames[i]));
            }

            return customers;
        }

        private static Customer CreateCustomer(int? tenantId, string nameSurname)
        {
            var entity = new Customer
            {
                FirstName = nameSurname.Split(' ')[0],
                LastName = nameSurname.Split(' ')[1],
                Age = RandomHelper.GetRandom(18, 60)
            };

            var prop = typeof (Customer).GetProperty("TenantId");
            prop?.SetValue(entity, tenantId);

            return entity;
        }

        public void Create()
        {
            var list = GetRandomCustomers(52, TenantId.Value);
            foreach (var item in list)
            {
                AddIfNotExists(item);
            }
        }

        private void AddIfNotExists(Customer item)
        {
            if (Table.Any(x => x.FirstName == item.FirstName && 
                        x.LastName == item.LastName))
            {
                return;
            }

            Insert(item);
            BatchContext.SaveChanges();
        }
    }
}
