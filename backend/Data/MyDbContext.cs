using Microsoft.EntityFrameworkCore;
using Shared.Models;
using Shared.DTOs;

namespace Backend.Data
{
      public class MyDbContext : DbContext
      {
            public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

            public DbSet<UserModel> Users { get; set; }
            public DbSet<PhoneModel> Phones { get; set; }
            public DbSet<CartItemModel> CartItems { get; set; }
            public DbSet<OfferModel> Offers { get; set; }
            public DbSet<OrderModel> Orders { get; set; }
            public DbSet<OrderItemModel> OrderItems { get; set; }
            public DbSet<ShoppingCartModel> ShoppingCarts { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                  base.OnModelCreating(modelBuilder);

                  // Configure UserModel
                  modelBuilder.Entity<UserModel>(entity =>
                  {
                        entity.HasKey(u => u.UserID);

                        entity.Property(u => u.Email)
                        .IsRequired()
                        .HasMaxLength(255);

                        entity.Property(u => u.PasswordHash)
                        .IsRequired()
                        .HasMaxLength(255);

                        entity.Property(u => u.FirstName)
                        .IsRequired()
                        .HasMaxLength(100);

                        entity.Property(u => u.LastName)
                        .IsRequired()
                        .HasMaxLength(100);

                        entity.Property(u => u.Role)
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasDefaultValue("Customer");

                        entity.Property(u => u.CreatedDate)
                        .IsRequired();

                        entity.Property(u => u.Address)
                        .HasMaxLength(255);

                        entity.Property(u => u.PhoneNumber)
                        .HasMaxLength(20);

                        // One-to-One: UserModel <-> ShoppingCartModel
                        entity.HasOne(u => u.ShoppingCart)
                        .WithOne(sc => sc.User)
                        .HasForeignKey<ShoppingCartModel>(sc => sc.UserID)
                        .OnDelete(DeleteBehavior.Cascade);

                        // One-to-Many: UserModel -> OfferModel
                        entity.HasMany(u => u.Offers)
                        .WithOne(o => o.User)
                        .HasForeignKey(o => o.UserID)
                        .OnDelete(DeleteBehavior.Cascade);

                        // One-to-Many: UserModel -> OrderModel
                        entity.HasMany(u => u.Orders)
                        .WithOne(o => o.User)
                        .HasForeignKey(o => o.UserID)
                        .OnDelete(DeleteBehavior.Cascade);
                  });

                  // Configure PhoneModel
                  modelBuilder.Entity<PhoneModel>(entity =>
                  {
                        entity.HasKey(p => p.PhoneID);

                        entity.Property(p => p.Brand)
                        .IsRequired()
                        .HasMaxLength(100);

                        entity.Property(p => p.Model)
                        .IsRequired()
                        .HasMaxLength(100);

                        entity.Property(p => p.Price)
                        .IsRequired()
                        .HasColumnType("decimal(18,2)");

                        entity.Property(p => p.Description)
                        .HasMaxLength(1000);

                        entity.Property(p => p.Condition)
                        .IsRequired()
                        .HasMaxLength(50);

                        entity.Property(p => p.StockQuantity)
                        .IsRequired();

                        // Relationships
                        entity.HasMany(p => p.CartItems)
                        .WithOne(ci => ci.Phone)
                        .HasForeignKey(ci => ci.PhoneID)
                        .OnDelete(DeleteBehavior.Cascade);

                        entity.HasMany(p => p.OrderItems)
                        .WithOne(oi => oi.Phone)
                        .HasForeignKey(oi => oi.PhoneID)
                        .OnDelete(DeleteBehavior.Cascade);

                        entity.HasMany(p => p.Offers)
                        .WithOne(o => o.Phone)
                        .HasForeignKey(o => o.PhoneID)
                        .OnDelete(DeleteBehavior.Cascade);
                  });

                  // Configure OfferModel
                  modelBuilder.Entity<OfferModel>(entity =>
                  {
                        entity.HasKey(o => o.OfferID);

                        entity.Property(o => o.PhoneBrand)
                        .IsRequired();

                        entity.Property(o => o.PhoneModel)
                        .IsRequired();

                        entity.Property(o => o.OriginalPrice)
                        .IsRequired()
                        .HasColumnType("decimal(18,2)");

                        entity.Property(o => o.PhoneAge)
                        .IsRequired();

                        entity.Property(o => o.OverallCondition)
                        .IsRequired();

                        entity.Property(o => o.BatteryLife)
                        .IsRequired();

                        entity.Property(o => o.ScreenCondition)
                        .IsRequired();

                        entity.Property(o => o.CameraCondition)
                        .IsRequired();

                        entity.Property(o => o.Status)
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasDefaultValue("Pending");

                        entity.Property(o => o.SubmissionDate)
                        .IsRequired();

                        // Relationships are configured in UserModel and PhoneModel
                  });

                  // Configure OrderModel
                  modelBuilder.Entity<OrderModel>(entity =>
                  {
                        entity.HasKey(o => o.OrderID);

                        entity.Property(o => o.OrderDate)
                        .IsRequired();

                        entity.Property(o => o.TotalPrice)
                        .IsRequired()
                        .HasColumnType("decimal(18,2)");

                        entity.Property(o => o.Status)
                        .IsRequired()
                        .HasMaxLength(50);

                        // Relationships
                        entity.HasMany(o => o.OrderItems)
                        .WithOne(oi => oi.Order)
                        .HasForeignKey(oi => oi.OrderID)
                        .OnDelete(DeleteBehavior.Cascade);
                  });

                  // Configure OrderItemModel
                  modelBuilder.Entity<OrderItemModel>(entity =>
                  {
                        entity.HasKey(oi => oi.OrderItemID);

                        entity.Property(oi => oi.Quantity)
                        .IsRequired();

                        entity.Property(oi => oi.Price)
                        .IsRequired()
                        .HasColumnType("decimal(18,2)");

                        // Relationships
                        entity.HasOne(oi => oi.Order)
                        .WithMany(o => o.OrderItems)
                        .HasForeignKey(oi => oi.OrderID)
                        .OnDelete(DeleteBehavior.Cascade);

                        entity.HasOne(oi => oi.Phone)
                        .WithMany(p => p.OrderItems)
                        .HasForeignKey(oi => oi.PhoneID)
                        .OnDelete(DeleteBehavior.Cascade);
                  });

                  // Configure CartItemModel
                  modelBuilder.Entity<CartItemModel>(entity =>
                  {
                        entity.HasKey(ci => ci.CartItemID);

                        entity.Property(ci => ci.Quantity)
                        .IsRequired();

                        // Relationships
                        entity.HasOne(ci => ci.ShoppingCart)
                        .WithMany(sc => sc.CartItems)
                        .HasForeignKey(ci => ci.CartID)
                        .OnDelete(DeleteBehavior.Cascade);

                        entity.HasOne(ci => ci.Phone)
                        .WithMany(p => p.CartItems)
                        .HasForeignKey(ci => ci.PhoneID)
                        .OnDelete(DeleteBehavior.Cascade);
                  });

                  // Configure ShoppingCartModel
                  modelBuilder.Entity<ShoppingCartModel>(entity =>
                  {
                        entity.HasKey(sc => sc.CartID);

                        entity.Property(sc => sc.UserID)
                        .IsRequired();

                        // Relationships
                        entity.HasOne(sc => sc.User)
                        .WithOne(u => u.ShoppingCart)
                        .HasForeignKey<ShoppingCartModel>(sc => sc.UserID)
                        .OnDelete(DeleteBehavior.Cascade);

                        entity.HasMany(sc => sc.CartItems)
                        .WithOne(ci => ci.ShoppingCart)
                        .HasForeignKey(ci => ci.CartID)
                        .OnDelete(DeleteBehavior.Cascade);
                  });

                  // Seed initial data
                  DataSeeder.Seed(modelBuilder);
            }
      }
}