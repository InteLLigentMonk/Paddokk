using System.Security.Claims;
using MediatR;
using Paddokk.Core.Features.Users.Commands.EnsureUserExists;

namespace Paddokk.Api.Middleware;

public class UserSyncMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, ISender mediator)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? context.User.FindFirst("sub")?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                var email = context.User.FindFirst(ClaimTypes.Email)?.Value
                    ?? context.User.FindFirst("email")?.Value;
                var fullName = context.User.FindFirst(ClaimTypes.Name)?.Value
                    ?? context.User.FindFirst("name")?.Value;
                var givenName = context.User.FindFirst(ClaimTypes.GivenName)?.Value
                    ?? context.User.FindFirst("given_name")?.Value;
                var familyName = context.User.FindFirst(ClaimTypes.Surname)?.Value
                    ?? context.User.FindFirst("family_name")?.Value;

                await mediator.Send(new EnsureUserExistsCommand(userId, email, fullName, givenName, familyName));
            }
        }

        await _next(context);
    }
}
