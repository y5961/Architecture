using ChineseAuctionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ChineseAuctionAPI.Data
{
    public class SaleContextDB : DbContext
    {
        public SaleContextDB(DbContextOptions<SaleContextDB> options) : base(options) { }
        public DbSet<User> Users => Set<User>();
        public DbSet<Gift> Gifts => Set<Gift>();
        public DbSet<Donor> Donors => Set<Donor>();
        public DbSet<OrdersGift> OrdersGift => Set<OrdersGift>();
        public DbSet<Order> OrdersOrders => Set<Order>();
        public DbSet<Package> Packages => Set<Package>();
        public DbSet<OrdersPackage> OrdersPackage => Set<OrdersPackage>();
        public DbSet<GiftCategory> GiftCategories => Set<GiftCategory>();

        public DbSet<Card> Cards => Set<Card>();
        public DbSet<Winner> winners => Set<Winner>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Order>()
          .HasIndex(o => o.IdUser)
          .HasFilter("[Status] = 0") // Status.Draft = 0 לפי enum
          .IsUnique();

        }

    }
}
