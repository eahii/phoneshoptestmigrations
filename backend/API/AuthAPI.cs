using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Shared.Models;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Backend.Api
{
    public static class AuthApi
    {
        public static void MapAuthApi(this WebApplication app)
        {
            app.MapPost("/api/auth/register", RegisterUser);
            app.MapPost("/api/auth/login", LoginUser);
            app.MapPut("/api/auth/promote/{id}", PromoteUser).RequireAuthorization("Admin");
        }

        // Käyttäjän rekisteröinti
        public static async Task<IResult> RegisterUser(UserModel user)
        {
            using var connection = new SqliteConnection("Data Source=UsedPhonesShop.db");
            await connection.OpenAsync();

            // Tarkista, onko käyttäjä jo olemassa
            var checkUserCmd = connection.CreateCommand();
            checkUserCmd.CommandText = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
            checkUserCmd.Parameters.AddWithValue("@Email", user.Email);

            var exists = Convert.ToInt32(await checkUserCmd.ExecuteScalarAsync()) > 0;
            if (exists)
            {
                return Results.BadRequest(new { Error = "Käyttäjä on jo olemassa." });
            }

            // Hashaa salasana
            user.PasswordHash = HashPassword(user.Password);

            // Lisää käyttäjä
            var insertCmd = connection.CreateCommand();
            insertCmd.CommandText = @"
                INSERT INTO Users (Email, PasswordHash, FirstName, LastName, Address, PhoneNumber, Role, CreatedDate)
                VALUES (@Email, @PasswordHash, @FirstName, @LastName, @Address, @PhoneNumber, 'Customer', CURRENT_TIMESTAMP)";
            insertCmd.Parameters.AddWithValue("@Email", user.Email);
            insertCmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            insertCmd.Parameters.AddWithValue("@FirstName", user.FirstName);
            insertCmd.Parameters.AddWithValue("@LastName", user.LastName);
            insertCmd.Parameters.AddWithValue("@Address", user.Address ?? (object)DBNull.Value);
            insertCmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber ?? (object)DBNull.Value);

            await insertCmd.ExecuteNonQueryAsync();

            return Results.Ok("Rekisteröinti onnistui.");
        }

        // Käyttäjän kirjautuminen
        public static async Task<IResult> LoginUser(LoginModel login)
        {
            using var connection = new SqliteConnection("Data Source=UsedPhonesShop.db");
            await connection.OpenAsync();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT UserID, PasswordHash, FirstName, LastName, Email, Role FROM Users WHERE Email = @Email";
            cmd.Parameters.AddWithValue("@Email", login.Email);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return Results.Unauthorized();
            }

            var storedHash = reader.GetString(1);
            if (!VerifyPassword(login.Password, storedHash))
            {
                return Results.Unauthorized();
            }

            // Generate JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("YourSecretKeyHere"); // Ensure this matches the one in Program.cs

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, reader.GetInt32(0).ToString()),
                    new Claim(ClaimTypes.Email, reader.GetString(4)),
                    new Claim(ClaimTypes.Role, reader.GetString(5))
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            return Results.Ok(new { Token = jwtToken });
        }

        // Promotes a user to Admin role
        private static async Task<IResult> PromoteUser(int id, HttpContext context)
        {
            // Ensure the requester has Admin role
            var user = context.User;
            if (!user.IsInRole("Admin"))
            {
                return Results.Forbid();
            }

            using var connection = new SqliteConnection("Data Source=UsedPhonesShop.db");
            await connection.OpenAsync();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Users SET Role = 'Admin' WHERE UserID = @UserID";
            cmd.Parameters.AddWithValue("@UserID", id);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            if (rowsAffected > 0)
            {
                return Results.Ok("Käyttäjä on edistetty Admin-rooliin.");
            }
            else
            {
                return Results.NotFound("Käyttäjää ei löytynyt.");
            }
        }

        // Hashaa salasanan SHA-256-algoritmilla
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        // Tarkistaa, vastaako salasana tallennettua hashia
        private static bool VerifyPassword(string password, string storedHash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == storedHash;
        }
    }
}
