using System;
using System.Collections.Generic;
using System.Text;
using CareStream.Models;
using CareStream.Models.RolesAndPermissions;
using Microsoft.EntityFrameworkCore;

namespace CareStream.Scheduler
{
    public class CareStreamContext : DbContext
    {
        public CareStreamContext(DbContextOptions<CareStreamContext> options) : base(options)
        {

        }
        public DbSet<BulkUserFile> BulkUserFiles { get; set; }
        public DbSet<BulkUser> BulkUsers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Role Seed Data

            modelBuilder.Entity<Role>().HasData(new Role { RoleId = 1, RoleSection = "Users", CreatedBy = "Admin", ModifiedBy = "Admin", CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now });
            modelBuilder.Entity<Role>().HasData(new Role { RoleId = 2, RoleSection = "Groups", CreatedBy = "Admin", ModifiedBy = "Admin", CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now });
            modelBuilder.Entity<Role>().HasData(new Role { RoleId = 3, RoleSection = "UserAttributes", CreatedBy = "Admin", ModifiedBy = "Admin", CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now });
            modelBuilder.Entity<Role>().HasData(new Role { RoleId = 4, RoleSection = "BulkOperations", CreatedBy = "Admin", ModifiedBy = "Admin", CreatedDate = DateTime.Now, ModifiedDate = DateTime.Now });

            #endregion
        }
    }
}
