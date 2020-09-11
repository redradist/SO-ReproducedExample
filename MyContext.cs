using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SOReproduce
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options) { }

        public DbSet<Item> Items { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<StoreItem> StoreItem { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<StoreItem>().HasKey(i => new { i.ItemId, i.StoreId });
        }
    }

    public class Store
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<StoreItem> Items { get; set; }

    }
    public class Item
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class StoreItem
    {
        public int ItemId { get; set; }
        public Item Item { get; set; }
        public int StoreId { get; set; }
        public Store Store { get; set; }
    }
}
