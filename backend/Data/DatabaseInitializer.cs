using Microsoft.Data.Sqlite;
using Shared.Models;
using System.Threading.Tasks;
using Backend.Api; // Adjust the namespace according to your project structure

namespace Backend.Data
{
    public static class DatabaseInitializer
    {
        // Tämä metodi alustaa tietokantataulut, jos niitä ei ole vielä olemassa
        public static async Task Initialize()
        {
            using (var connection = new SqliteConnection("Data Source=UsedPhonesShop.db"))
            {
                await connection.OpenAsync(); // Yhteyden avaaminen asynkronisesti

                // Luodaan Phones-taulu (tallentaa puhelinten tiedot)
                var createPhonesTable = connection.CreateCommand();
                createPhonesTable.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Phones (
                        PhoneID INTEGER PRIMARY KEY AUTOINCREMENT, -- PK, puhelimen yksilöllinen tunniste
                        Brand TEXT NOT NULL,                         -- Puhelimen merkki
                        Model TEXT NOT NULL,                         -- Puhelimen malli
                        Price REAL NOT NULL,                         -- Puhelimen hinta
                        Description TEXT,                            -- Puhelimen kuvaus
                        Condition TEXT NOT NULL,                     -- Puhelimen kunto
                        StockQuantity INTEGER                        -- Varastossa oleva määrä
                    )";
                await createPhonesTable.ExecuteNonQueryAsync(); // Taulun luonti asynkronisesti

                // Luodaan Users-taulu (tallentaa käyttäjien tiedot)
                var createUsersTable = connection.CreateCommand();
                createUsersTable.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        UserID INTEGER PRIMARY KEY AUTOINCREMENT, -- PK, käyttäjän yksilöllinen tunniste
                        Email TEXT NOT NULL,                       -- Käyttäjän sähköpostiosoite
                        PasswordHash TEXT NOT NULL,                -- Käyttäjän salasanan hash
                        FirstName TEXT NOT NULL,                   -- Käyttäjän etunimi
                        LastName TEXT NOT NULL,                    -- Käyttäjän sukunimi
                        Address TEXT,                              -- Käyttäjän osoite
                        PhoneNumber TEXT,                          -- Käyttäjän puhelinnumero
                        Role TEXT NOT NULL DEFAULT 'Customer',     -- Käyttäjän rooli
                        CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP -- Luontipäivämäärä
                    )";
                await createUsersTable.ExecuteNonQueryAsync(); // Taulun luonti asynkronisesti

                // Lisää Admin user if not exists
                var checkAdminCommand = connection.CreateCommand();
                checkAdminCommand.CommandText = "SELECT COUNT(1) FROM Users WHERE Email = 'admin@shop.com'";
                var adminExists = Convert.ToInt32(await checkAdminCommand.ExecuteScalarAsync()) > 0;
                if (!adminExists)
                {
                    var insertAdminCommand = connection.CreateCommand();
                    insertAdminCommand.CommandText = @"
                        INSERT INTO Users (Email, PasswordHash, FirstName, LastName, Role)
                        VALUES (@Email, @PasswordHash, @FirstName, @LastName, 'Admin')";
                    insertAdminCommand.Parameters.AddWithValue("@Email", "admin@shop.com");
                    insertAdminCommand.Parameters.AddWithValue("@PasswordHash", AuthApi.HashPassword("Admin@123")); // Ensure to change the password
                    insertAdminCommand.Parameters.AddWithValue("@FirstName", "Admin");
                    insertAdminCommand.Parameters.AddWithValue("@LastName", "User");
                    await insertAdminCommand.ExecuteNonQueryAsync();
                }

                // ... (Rest of the table creations)
            }
        }
    }
}
