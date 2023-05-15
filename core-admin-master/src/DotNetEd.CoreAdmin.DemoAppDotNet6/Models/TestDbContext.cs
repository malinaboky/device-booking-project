﻿using DotNetEd.CoreAdmin.DemoAppDotNet6.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetEd.CoreAdmin.DemoApp.Models
{
    public class TestDbContext : DbContext
    {
        public DbSet<TestEntity> TestEntities { get; set; }

        public DbSet<TestAutogeneratedKeyEntity> TestAutogeneratedKeyEntities { get; set; }

        public DbSet<TestEntityWithImage> TestEntitiesWithImages { get; set; }

        public TestDbContext(DbContextOptions<TestDbContext> contextOptions) : base(contextOptions)
        {
            this.Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var seedData = new List<TestEntity>();
            foreach(var i in Enumerable.Range(0, 2000))
            {
                seedData.Add(new TestEntity() { Id = Guid.NewGuid(), Name = "Test entity " + i, Title = "Test title " + i, Price = new Random().NextDouble() });
            }

            modelBuilder.Entity<TestEntity>().HasData(seedData);

            var seedDataImages = new List<TestEntityWithImage>();

            seedDataImages.Add(new TestEntityWithImage() { Id = Guid.NewGuid(), Name = "Handsome person", Image = System.IO.File.ReadAllBytes("DemoAssets/ed-100.png") });
        
            modelBuilder.Entity<TestEntityWithImage>().HasData(seedDataImages);
        }
    }
}