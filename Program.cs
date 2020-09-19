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


                await using (var transaction = context.Database.BeginTransaction())
                {
                    await context.Items.AddAsync(item1Entity);
                    await context.Items.AddAsync(item2Entity);
                    await context.Stores.AddAsync(storeEntity);

                    await context.SaveChangesAsync();

                    var storeItem1 = new StoreItemEntity()
                    {
                        ItemId = item1Entity.Id,
                        Item = item1Entity,
                        StoreId = storeEntity.Id,
                        Store = storeEntity,
                    };
                    var storesForItem1 = new List<StoreItemEntity>()
                    {
                        storeItem1
                    };
                    var storeItem2 = new StoreItemEntity()
                    {
                        ItemId = item2Entity.Id,
                        Item = item2Entity,
                        StoreId = storeEntity.Id,
                        Store = storeEntity,
                    };
                    var storesForItem2 = new List<StoreItemEntity>()
                    {
                        storeItem2
                    };
                    var itemsForStore = new List<StoreItemEntity>()
                    {
                        storeItem1,
                        storeItem2
                    };
                    item1Entity.Stores = storesForItem1;
                    await context.SaveChangesAsync();
                    item2Entity.Stores = storesForItem2;
                    await context.SaveChangesAsync();
                    storeEntity.Items = itemsForStore;
                    await context.SaveChangesAsync();
                    
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
