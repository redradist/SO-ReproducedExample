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
            string ipAddress = "localhost";
            short port = 5432;
            DbContextOptionsBuilder<MyContext> optionsBuilder = new DbContextOptionsBuilder<MyContext>()
                .UseNpgsql($"Host={ipAddress};Port={port};Database=tests;Username=postgres;Password=postgres");
            
            return new MyContext();
        }
        
        static async Task Main(string[] args)
        {
            var program = new Program();
            using (var context = program.CreateDbContext(null))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                
                ItemEntity item1Entity = new ItemEntity()
                {
                    Name = "Item1"
                };
                ItemEntity item2Entity = new ItemEntity()
                {
                    Name = "Item2"
                };
                StoreEntity storeEntity = new StoreEntity()
                {
                    Name = "Store1"
                };
                var storesForItem1 = new List<StoreItemEntity>()
                {
                    new StoreItemEntity()
                    {
                        ItemId = item1Entity.Id,
                        Item = item1Entity,
                        Store = storeEntity,
                    }
                };
                var storesForItem2 = new List<StoreItemEntity>()
                {
                    new StoreItemEntity()
                    {
                        ItemId = item2Entity.Id,
                        Item = item2Entity,
                        Store = storeEntity,
                    }
                };
                var itemsForStore = new List<StoreItemEntity>()
                {
                    new StoreItemEntity()
                    {
                        Item = item1Entity,
                        StoreId = storeEntity.Id,
                        Store = storeEntity,
                    },
                    new StoreItemEntity()
                    {
                        Item = item2Entity,
                        StoreId = storeEntity.Id,
                        Store = storeEntity,
                    }
                };
                item1Entity.Stores = storesForItem1;
                item2Entity.Stores = storesForItem2;
                storeEntity.Items = itemsForStore;

                var conn = new ConnEntity()
                {
                    Name = "Hr",
                };

                using (var transaction = context.Database.BeginTransaction())
                {
                    if (storeEntity.Id == 0)
                    {
                        await context.Stores.AddAsync(storeEntity);
                    }
                    else
                    {
                        context.Stores.Update(storeEntity);
                    }
                    await context.SaveChangesAsync();
                    transaction.Commit();
                }
            }
        }
    }
}
