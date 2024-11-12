using Microsoft.EntityFrameworkCore;
using Shared.Models; // Adjust the namespace based on where your models are located

namespace Backend.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        public DbSet<UserModel> Users { get; set; }
        // Add other DbSets for your entities here, e.g.:
        // public DbSet<CartItemModel> CartItems { get; set; }
        // public DbSet<OrderModel> Orders { get; set; }
    }
}
