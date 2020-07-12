using System;
using System.Collections.Generic;
using System.Text;
using CareStream.Models;
using CareStream.Models.Dealer;
using Microsoft.EntityFrameworkCore;


namespace CareStream.Utility
{


    public class CosmosDbContext : DbContext
    {
        public DbSet<ProductFamilyModel> productFamily { get; set; }
        public DbSet<AssignedDealerModel> assignedDealerModels { get; set; }
        public DbSet<DealerModel> dealers { get; set; }
        public DbSet<AssignedProductFamilyModel> assignedProductFamilyModels { get; set; }
        public DbSet<DeletedDealerModel> deletedDealerModels { get; set; }
        public DbSet<DeletedProductFamily> deletedProductFamilies { get; set; }
        public DbSet<DeletedDealerProductFamilyModel> deletedDealerProductFamilyModels { get; set; }
        public DbSet<FileDetails> fileDetails { get; set; }

        public CosmosDbContext(DbContextOptions<CosmosDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // the container name
            modelBuilder.HasDefaultContainer("Dealers");
            modelBuilder.Entity<ProductFamilyModel>().OwnsMany(p => p.assignedDealerModels);
            modelBuilder.Entity<DealerModel>().OwnsMany(x => x.assignedProductFamilyModels);
            modelBuilder.Entity<DeletedDealerModel>().OwnsMany(d => d.deletedDealerProductFamilyModels);
            modelBuilder.Entity<DeletedProductFamily>().OwnsMany(pf => pf.deletedProductFamilyDealerModels);
        }
    }
}

