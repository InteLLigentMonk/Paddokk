using System.Security.Claims;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Paddokk.Core.Models.Entities;

namespace API.Middleware
{
    public class UserSyncMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context, PaddokkDbContext dbContext)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var email = context.User.FindFirst(ClaimTypes.Email)?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    // Kolla om användaren finns
                    var exists = await dbContext.Users
                        .AnyAsync(u => u.Id == userId);

                    if (!exists)
                    {
                        // Skapa ny användare
                        var newUser = new ApplicationUser
                        {
                            Id = userId,
                            Email = email,
                            DisplayName = email?.Split('@')[0] ?? "User",
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        dbContext.Users.Add(newUser);
                        await dbContext.SaveChangesAsync();
                    }
                }
            }

            await _next(context);
        }
    }
}
