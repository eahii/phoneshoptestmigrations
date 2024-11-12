using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Shared.Models;
using System.Text.Json;

namespace Backend.Api
{
    public static class CartApi
    {
        public static void MapCartApi(this WebApplication app)
        {
            app.MapGet("/api/cart", (Func<HttpContext, Task<IResult>>)GetCart).RequireAuthorization();
            app.MapPost("/api/cart/items", (Func<CartItemModel, HttpContext, Task<IResult>>)AddToCart).RequireAuthorization();
            app.MapPut("/api/cart/items/{id}", (Func<int, JsonElement, Task<IResult>>)UpdateCartItem).RequireAuthorization();
            app.MapDelete("/api/cart/items/{id}", RemoveFromCart).RequireAuthorization();
        }

        private static async Task<IResult> GetCart(HttpContext context)
        {
            // TODO: Get actual user ID from authentication
            int userId = 1; // Temporary hardcoded user ID

            try
            {
                using var connection = new SqliteConnection("Data Source=UsedPhonesShop.db");
                await connection.OpenAsync();

                // First, ensure user has a cart
                var cartId = await EnsureUserHasCart(connection, userId);

                // Get cart items with phone details
                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT 
                        ci.CartItemID, 
                        ci.CartID, 
                        ci.Quantity,
                        p.PhoneID,
                        p.Brand,
                        p.Model,
                        p.Price,
                        p.Description,
                        p.Condition,
                        p.StockQuantity
                    FROM CartItems ci
                    JOIN Phones p ON ci.PhoneID = p.PhoneID
                    WHERE ci.CartID = @CartID";
                command.Parameters.AddWithValue("@CartID", cartId);

                var cartItems = new List<CartItemWithPhone>();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    cartItems.Add(new CartItemWithPhone
                    {
                        CartItemID = reader.GetInt32(0),
                        CartID = reader.GetInt32(1),
                        Quantity = reader.GetInt32(2),
                        Phone = new PhoneModel
                        {
                            PhoneID = reader.GetInt32(3),
                            Brand = reader.GetString(4),
                            Model = reader.GetString(5),
                            Price = reader.GetDecimal(6),
                            Description = reader.GetString(7),
                            Condition = reader.GetString(8),
                            StockQuantity = reader.GetInt32(9)
                        }
                    });
                }

                return Results.Ok(cartItems);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error getting cart: {ex.Message}");
            }
        }

        private static async Task<IResult> AddToCart(CartItemModel cartItem, HttpContext context)
        {
            // TODO: Get actual user ID from authentication
            int userId = 1; // Temporary hardcoded user ID

            try
            {
                using var connection = new SqliteConnection("Data Source=UsedPhonesShop.db");
                await connection.OpenAsync();

                // Ensure user has a cart and get cart ID
                var cartId = await EnsureUserHasCart(connection, userId);
                cartItem.CartID = cartId;

                // Check if item already exists in cart
                var checkCommand = connection.CreateCommand();
                checkCommand.CommandText = @"
                    SELECT CartItemID, Quantity 
                    FROM CartItems 
                    WHERE CartID = @CartID AND PhoneID = @PhoneID";
                checkCommand.Parameters.AddWithValue("@CartID", cartId);
                checkCommand.Parameters.AddWithValue("@PhoneID", cartItem.PhoneID);

                using var reader = await checkCommand.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    // Item exists, update quantity
                    var existingItemId = reader.GetInt32(0);
                    var currentQuantity = reader.GetInt32(1);
                    reader.Close();

                    var updateCommand = connection.CreateCommand();
                    updateCommand.CommandText = @"
                        UPDATE CartItems 
                        SET Quantity = @Quantity 
                        WHERE CartItemID = @CartItemID";
                    updateCommand.Parameters.AddWithValue("@Quantity", currentQuantity + cartItem.Quantity);
                    updateCommand.Parameters.AddWithValue("@CartItemID", existingItemId);
                    await updateCommand.ExecuteNonQueryAsync();
                }
                else
                {
                    reader.Close();
                    // Add new item to cart
                    var insertCommand = connection.CreateCommand();
                    insertCommand.CommandText = @"
                        INSERT INTO CartItems (CartID, PhoneID, Quantity)
                        VALUES (@CartID, @PhoneID, @Quantity)";
                    insertCommand.Parameters.AddWithValue("@CartID", cartItem.CartID);
                    insertCommand.Parameters.AddWithValue("@PhoneID", cartItem.PhoneID);
                    insertCommand.Parameters.AddWithValue("@Quantity", cartItem.Quantity);
                    await insertCommand.ExecuteNonQueryAsync();
                }

                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error adding to cart: {ex.Message}");
            }
        }

        private static async Task<IResult> UpdateCartItem(int id, JsonElement quantityData)
        {
            try
            {
                var quantity = quantityData.GetProperty("quantity").GetInt32();
                using var connection = new SqliteConnection("Data Source=UsedPhonesShop.db");
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = "UPDATE CartItems SET Quantity = @Quantity WHERE CartItemID = @CartItemID";
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@CartItemID", id);

                await command.ExecuteNonQueryAsync();
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error updating cart item: {ex.Message}");
            }
        }

        private static async Task<IResult> RemoveFromCart(int id)
        {
            try
            {
                using var connection = new SqliteConnection("Data Source=UsedPhonesShop.db");
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM CartItems WHERE CartItemID = @CartItemID";
                command.Parameters.AddWithValue("@CartItemID", id);

                await command.ExecuteNonQueryAsync();
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error removing from cart: {ex.Message}");
            }
        }

        private static async Task<int> EnsureUserHasCart(SqliteConnection connection, int userId)
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT CartID FROM ShoppingCart WHERE UserID = @UserID";
            command.Parameters.AddWithValue("@UserID", userId);

            var cartId = await command.ExecuteScalarAsync();

            if (cartId == null)
            {
                command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO ShoppingCart (UserID) 
                    VALUES (@UserID);
                    SELECT last_insert_rowid();";
                command.Parameters.AddWithValue("@UserID", userId);
                cartId = await command.ExecuteScalarAsync();
            }

            return Convert.ToInt32(cartId);
        }
    }
}