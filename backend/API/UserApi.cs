using Microsoft.AspNetCore.Builder;
using Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Api
{
    public static class UserApi
    {
        public static void MapUserApi(this WebApplication app)
        {
            app.MapGet("/api/users", GetAllUsers)
               .RequireAuthorization("Admin"); // Only Admins can access
        }

        private static async Task<IResult> GetAllUsers(MyDbContext dbContext)
        {
            try
            {
                var users = await dbContext.Users
                                           .Select(u => new
                                           {
                                               u.UserID,
                                               u.Email,
                                               u.FirstName,
                                               u.LastName,
                                               u.Role
                                           })
                                           .ToListAsync();

                return Results.Ok(users);
            }
            catch (Exception ex)
            {
                // Implement proper logging here
                return Results.Problem("An error occurred while fetching users.");
            }
        }
    }
}