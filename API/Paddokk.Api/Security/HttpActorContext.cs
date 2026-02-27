using Paddokk.Api.Extensions;
using Paddokk.Core.Interfaces;

namespace Paddokk.Api.Security;

public sealed class HttpActorContext(IHttpContextAccessor httpContextAccessor) : IActorResolver
{
    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

    public string UserId =>
        httpContextAccessor.HttpContext?.User.GetUserId()
        ?? throw new UnauthorizedAccessException("No authenticated user");
}