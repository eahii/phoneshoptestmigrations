using Microsoft.AspNetCore.Builder;
using Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Api
{
    public static class OfferApi
    {
        public static void MapOfferApi(this WebApplication app)
        {
            app.MapPost("/api/offers", AddOffer)
               .RequireAuthorization(); // Require authenticated users

            app.MapGet("/api/offers/pending", GetPendingOffers)
               .RequireAuthorization("Admin"); // Only Admins can access

            app.MapPost("/api/offers/accept/{id}", AcceptOffer)
               .RequireAuthorization("Admin"); // Only Admins can accept

            app.MapPost("/api/offers/decline/{id}", DeclineOffer)
               .RequireAuthorization("Admin"); // Only Admins can decline
        }

        private static async Task<IResult> AddOffer(OfferModel offer, MyDbContext dbContext)
        {
            try
            {
                dbContext.Offers.Add(offer);
                await dbContext.SaveChangesAsync();
                return Results.Ok("Offer submitted successfully.");
            }
            catch (Exception ex)
            {
                // Implement logging here
                return Results.Problem("An error occurred while submitting the offer.");
            }
        }

        private static async Task<IResult> GetPendingOffers(MyDbContext dbContext)
        {
            try
            {
                var pendingOffers = await dbContext.Offers
                    .Where(o => o.Status == "Pending")
                    .ToListAsync();

                return Results.Ok(pendingOffers);
            }
            catch (Exception ex)
            {
                // Implement logging here
                return Results.Problem("An error occurred while fetching pending offers.");
            }
        }

        private static async Task<IResult> AcceptOffer(int id, MyDbContext dbContext)
        {
            try
            {
                var offer = await dbContext.Offers.FindAsync(id);
                if (offer == null)
                {
                    return Results.NotFound();
                }

                offer.Status = "Approved";
                await dbContext.SaveChangesAsync();

                return Results.Ok("Offer approved.");
            }
            catch (Exception ex)
            {
                // Implement logging here
                return Results.Problem("An error occurred while approving the offer.");
            }
        }

        private static async Task<IResult> DeclineOffer(int id, MyDbContext dbContext)
        {
            try
            {
                var offer = await dbContext.Offers.FindAsync(id);
                if (offer == null)
                {
                    return Results.NotFound();
                }

                offer.Status = "Rejected";
                await dbContext.SaveChangesAsync();

                return Results.Ok("Offer rejected.");
            }
            catch (Exception ex)
            {
                // Implement logging here
                return Results.Problem("An error occurred while rejecting the offer.");
            }
        }
    }
}