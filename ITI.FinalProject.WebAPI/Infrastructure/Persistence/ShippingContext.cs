using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ShippingContext:IdentityDbContext<ApplicationUser,ApplicationRoles,string>
    {
        public ShippingContext(DbContextOptions<ShippingContext> contextOptions) : base(contextOptions)
        { }

        public DbSet<Branch> Branches { get; set; } 
        public DbSet<City> Cities { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Governorate> Governorates { get; set;}
        public DbSet<GovernorateRepresentatives> GovernorateRepresentatives { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Representative> Representatives { get; set; }
        public DbSet<RolePowers> RolePowers { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<SpecialPackages> SpecialPackages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<RolePowers>().HasKey("TableName", "RoleId");
            builder.Entity<GovernorateRepresentatives>().HasKey("representativeId", "governorateId");
            builder.Entity<SpecialPackages>().HasKey("MerchantId", "cityId");

            builder.Entity<City>()
                .HasOne(c => c.governorate)
                .WithMany(g => g.cities)
                .HasForeignKey(c => c.governorateId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Branch>()
                .HasOne(b => b.city)
                .WithOne(c => c.branch)
                .HasForeignKey<Branch>(b => b.cityId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Order>()
                .HasOne(o => o.city)
                .WithMany(c => c.cityOrders)
                .HasForeignKey(o => o.CityId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Order>()
                .HasOne(o => o.governorate)
                .WithMany(g => g.governorateOrders)
                .HasForeignKey(o => o.GovernorateId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Order>()
                .HasOne(o => o.merchant)
                .WithMany(m => m.orders)
                .HasForeignKey(o => o.MerchantId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Order>()
                .HasOne(o => o.branch)
                .WithMany(b => b.branchOrders)
                .HasForeignKey(o => o.BranchId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Order>()
                .HasOne(o => o.representative)
                .WithMany(r => r.representativeOrders)
                .HasForeignKey(o => o.RepresentativeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Merchant>()
                .HasOne(m => m.city)
                .WithMany(c => c.cityMerchants)
                .HasForeignKey(m => m.CityId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<GovernorateRepresentatives>()
                .HasOne(gr=> gr.governorate)
                .WithMany(g => g.representatives)
                .HasForeignKey(m => m.governorateId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<GovernorateRepresentatives>()
                .HasOne(gr => gr.representative)
                .WithMany(r => r.governorates)
                .HasForeignKey(m => m.representativeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Product>()
                .HasOne(p => p.order)
                .WithMany(o => o.Products)
                .HasForeignKey(m => m.OrderId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<SpecialPackages>()
                .HasOne(s => s.governoratePackages)
                .WithMany(g => g.specialPackages)
                .HasForeignKey(s => s.governorateId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<SpecialPackages>()
                .HasOne(s => s.merchantSpecialPackage)
                .WithMany(m => m.SpecialPackages)
                .HasForeignKey(s => s.MerchantId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<SpecialPackages>()
                .HasOne(s => s.cityPackages)
                .WithMany(c => c.citySpecialPackages)
                .HasForeignKey(s => s.cityId)
                .OnDelete(DeleteBehavior.NoAction);

        }

    }
}
