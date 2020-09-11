using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOReproduce
{
    class Program : IDesignTimeDbContextFactory<MyContext>
    {
        public MyContext CreateDbContext(string[] args)
        {           
            DbContextOptionsBuilder<MyContext> optionsBuilder = new DbContextOptionsBuilder<MyContext>()
                .UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog =SOReproducteDb; Integrated Security=True;Trusted_Connection=True;");

            return new MyContext(optionsBuilder.Options);
        }
        static async Task Main(string[] args)
        {
            var program = new Program();
            using (var context = program.CreateDbContext(null))
            {
                Item item = new Item()
                {
                    Name = "Item1"
                };
                using (var transaction = context.Database.BeginTransaction())
                {
                    await context.Items.AddAsync(item); // The item is stored in store StoreItem
                    await context.SaveChangesAsync();

                    transaction.Commit();
                }
                Store store = new Store()
                {
                    Name = "Store1"
                };
                store.Items = new List<StoreItem>()
                {
                    new StoreItem()
                    {
                        ItemId = item.Id,
                        Item = item,
                        Store = store,
                    }
                };
                using (var transaction = context.Database.BeginTransaction())
                {
                    await context.Stores.AddAsync(store);
                    await context.SaveChangesAsync();
                    transaction.Commit();
                }
            }
        }
    }
}
