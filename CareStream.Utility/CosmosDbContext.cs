using System;
using System.Collections.Generic;
using System.Text;
using CareStream.Models;
using Microsoft.EntityFrameworkCore;


namespace CareStream.Utility
{


    public class CosmosDbContext : DbContext
    {
        public DbSet<ProductFamilyModel> productFamily { get; set; }
        public DbSet<DealerModel> dealers { get; set; }
        public DbSet<AssignedProductFamilyModel> assignedProductFamilyModels { get; set; }
        public DbSet<DeletedDealerModel> deletedDealerModels { get; set; }
        public DbSet<DeletedDealerProductFamilyModel> deletedDealerProductFamilyModels { get; set; }
        //public DbSet<DealerDeleted> dealerDeleted { get; set; }


        public CosmosDbContext(DbContextOptions<CosmosDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // the container name
            modelBuilder.HasDefaultContainer("Dealers");
            //modelBuilder.Entity<DealerModel>().HasKey(k => k.DealerId);
            modelBuilder.Entity<DealerModel>().OwnsMany(x => x.assignedProductFamilyModels);
            modelBuilder.Entity<DeletedDealerModel>().OwnsMany(d => d.deletedDealerProductFamilyModels);

            // ProfileMaster has many educations and Many Experiences
            //modelBuilder.Entity<ProductFamilyCosmos>().HasKey(k => k.ProductFamilyId);
            //modelBuilder.Entity<DealerModelCosmos>().HasKey(k => k.DealerId);
            //modelBuilder.Entity<ProductFamilyCosmos>().HasMany(e => e.dealerModelCosmos);
            //modelBuilder.Entity<DealerDeleted>().HasKey(k => k.DealerId);
            //modelBuilder.Entity().OwnsMany(e => e.Experience);
        }
    }
}

