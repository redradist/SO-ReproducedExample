using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SOReproduce
{
    public class MyContext : DbContext
    {
        public MyContext() { }

        public DbSet<ItemEntity> Items { get; set; }
        public DbSet<StoreEntity> Stores { get; set; }
        public DbSet<ConnEntity> Conns { get; set; }
        public DbSet<ConnItemEntity> ConnItems { get; set; }
        public DbSet<StoreItemEntity> StoreItem { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            string ipAddress = "localhost";
            short port = 5432;
            optionsBuilder.UseNpgsql($"Host={ipAddress};Port={port};Database=tests;Username=postgres;Password=postgres");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ItemEntity>(entity =>
            {
                entity.Property(i => i.Name).IsRequired();
                entity.HasIndex(i => i.Name).IsUnique();
            });
            
            builder.Entity<StoreEntity>(entity =>
            {
                entity.Property(i => i.Name).IsRequired();
                entity.HasIndex(i => i.Name).IsUnique();
            });
            
            builder.Entity<ConnEntity>(entity =>
            {
                entity.Property(i => i.Name).IsRequired();
                entity.HasIndex(i => i.Name).IsUnique();
            });
            
            builder.Entity<StoreItemEntity>(entity =>
            {
                entity
                    .HasKey(i => new { i.ItemId, i.StoreId });
                entity
                    .HasOne(l => l.Item)
                    .WithMany(s => s.Stores)
                    .HasForeignKey(l => l.ItemId);
                entity
                    .HasOne(s => s.Store)
                    .WithMany(s => s.Items)
                    .HasForeignKey(s => s.StoreId); 
            });
            
            builder.Entity<ConnItemEntity>(entity =>
            {
                entity.HasKey(sc => new { sc.ItemId, sc.Name, sc.ConnId });
                entity
                    .Property(sc => sc.Name)
                    .IsRequired();
            });
            
            // Description of Stuffs
            builder.HasSequence<int>("stores_id_seq");
            builder.Entity<StoreEntity>()
                .Property(s => s.Id)
                .HasDefaultValueSql("nextval('stores_id_seq'::regclass)");
            
            builder.HasSequence<int>("items_id_seq");
            builder.Entity<ItemEntity>()
                .Property(s => s.Id)
                .HasDefaultValueSql("nextval('items_id_seq'::regclass)");
        }
    }

    public class StoreEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<StoreItemEntity> Items { get; set; }
    }
    
    public class ItemEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<ConnItemEntity> Conns { get; set; } = new List<ConnItemEntity>();
        public IList<StoreItemEntity> Stores { get; set; } = new List<StoreItemEntity>();
    }
    
    public class StoreItemEntity
    {
        public int ItemId { get; set; }
        public ItemEntity Item { get; set; }
        public int StoreId { get; set; }
        public StoreEntity Store { get; set; }
    }
    
    public class ConnEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    
    public class ConnItemEntity
    {
        public int ItemId { get; set; }
        public int ConnId { get; set; }
        public string Name { get; set; }
    }
}
