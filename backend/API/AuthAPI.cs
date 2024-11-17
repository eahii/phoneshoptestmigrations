using Microsoft.AspNetCore.Builder;
using Shared.DTOs;
using Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Backend.Api
{
    public static class AuthApi
    {
        public static void MapAuthApi(this WebApplication app)
        {
            app.MapPost("/api/auth/register", RegisterUser)
               .AllowAnonymous();

            app.MapPost("/api/auth/login", LoginUser)
               .AllowAnonymous();

            app.MapPut("/api/auth/promote/{id}", PromoteUser)
               .RequireAuthorization("Admin");
        }

        // User Registration
        public static async Task<IResult> RegisterUser(
            RegisterModel register,
            MyDbContext dbContext,
            ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("AuthAPI.RegisterUser");
            try
            {
                // Validate the incoming model
                if (!Validator.TryValidateObject(register, new ValidationContext(register), null, true))
                {
                    logger.LogWarning("Registration attempt failed: Invalid registration data.");
                    return Results.BadRequest(new { Error = "Invalid registration data." });
                }

                // Check if user already exists
                var existingUser = await dbContext.Users
                    .FirstOrDefaultAsync(u => u.Email == register.Email);
                if (existingUser != null)
                {
                    logger.LogWarning("Registration attempt failed: User with email {Email} already exists.", register.Email);
                    return Results.BadRequest(new { Error = "User already exists." });
                }

                // Create new UserModel instance
                var user = new UserModel
                {
                    Email = register.Email,
                    PasswordHash = HashPassword(register.Password),
                    FirstName = register.FirstName,
                    LastName = register.LastName,
                    Role = "Customer",
                    CreatedDate = DateTime.UtcNow,
                    Address = register.Address,
                    PhoneNumber = register.PhoneNumber
                };

                // Add user to the database
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();

                logger.LogInformation("New user registered with email {Email}.", register.Email);
                return Results.Ok("Registration successful.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during user registration.");
                return Results.Problem("An error occurred while processing your request.");
            }
        }

        // User Login
        public static async Task<IResult> LoginUser(
            LoginModel login,
            MyDbContext dbContext,
            IConfiguration config,
            ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("AuthAPI.LoginUser");
            try
            {
                // Validate the incoming model
                if (!Validator.TryValidateObject(login, new ValidationContext(login), null, true))
                {
                    logger.LogWarning("Login attempt failed: Invalid login data.");
                    return Results.BadRequest(new { Error = "Invalid login data." });
                }

                var user = await dbContext.Users
                    .FirstOrDefaultAsync(u => u.Email == login.Email);
                if (user == null)
                {
                    logger.LogWarning("Login attempt failed: User with email {Email} not found.", login.Email);
                    return Results.Unauthorized();
                }

                if (!VerifyPassword(login.Password, user.PasswordHash))
                {
                    logger.LogWarning("Login attempt failed: Incorrect password for email {Email}.", login.Email);
                    return Results.Unauthorized();
                }

                // Generate JWT Token
                var token = GenerateJwtToken(user, config);

                logger.LogInformation("User with email {Email} logged in successfully.", login.Email);
                return Results.Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during user login.");
                return Results.Problem("An error occurred while processing your request.");
            }
        }

        // User Promotion
        public static async Task<IResult> PromoteUser(
            int id,
            MyDbContext dbContext,
            ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("AuthAPI.PromoteUser");
            try
            {
                var user = await dbContext.Users.FindAsync(id);
                if (user == null)
                {
                    logger.LogWarning("Promotion attempt failed: User with ID {UserID} not found.", id);
                    return Results.NotFound(new { Error = "User not found." });
                }

                if (user.Role == "Admin")
                {
                    logger.LogInformation("User with ID {UserID} is already an Admin.", id);
                    return Results.Ok("User is already an Admin.");
                }

                user.Role = "Admin";
                await dbContext.SaveChangesAsync();

                logger.LogInformation("User with ID {UserID} has been promoted to Admin.", id);
                return Results.Ok("User promoted to Admin successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during user promotion.");
                return Results.Problem("An error occurred while processing your request.");
            }
        }

        // Password Hashing
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        // Password Verification
        private static bool VerifyPassword(string password, string storedHash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == storedHash;
        }

        // JWT Token Generation
        private static string GenerateJwtToken(UserModel user, IConfiguration config)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role) // Ensure Role claim is included
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}