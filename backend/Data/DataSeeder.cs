using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Backend.Data
{
    public static class DataSeeder
    {
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>().HasData(new UserModel
            {
                UserID = 1,
                Email = "admin@shop.com",
                PasswordHash = HashPassword("Admin@123"),
                FirstName = "Admin",
                LastName = "User",
                Role = "Admin",
                CreatedDate = new DateTime(2023, 1, 1),
                Address = "Admin Address",
                PhoneNumber = "1234567890"
            });
        }
    }
}
