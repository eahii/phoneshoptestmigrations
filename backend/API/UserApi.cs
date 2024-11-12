using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Api
{
    public static class UserApi
    {
        public static void MapUserApi(this WebApplication app)
        {
            app.MapGet("/api/users", GetAllUsers)
               .RequireAuthorization("Admin"); // Only Admins can access
        }

        private static async Task<IResult> GetAllUsers(HttpContext context)
        {
            var users = new List<UserModel>();

            using (var connection = new SqliteConnection("Data Source=UsedPhonesShop.db"))
            {
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT UserID, Email, FirstName, LastName, Role FROM Users";

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(new UserModel
                        {
                            UserID = reader.GetInt32(0),
                            Email = reader.GetString(1),
                            FirstName = reader.GetString(2),
                            LastName = reader.GetString(3),
                            Role = reader.GetString(4)
                        });
                    }
                }
            }

            return Results.Ok(users);
        }
    }
}
