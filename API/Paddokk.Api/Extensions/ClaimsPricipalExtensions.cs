using System.Security.Claims;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        // Better-auth typically uses "sub" (subject) claim for user ID
        // But can also use "id" or "userId" depending on configuration
        var userIdClaim = user.FindFirst("sub")?.Value 
            ?? user.FindFirst("id")?.Value 
            ?? user.FindFirst("userId")?.Value
            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("User ID not found in token claims");

        return userIdClaim;
    }

    public static string GetUsername(this ClaimsPrincipal user)
    {
        return user.FindFirst("username")?.Value 
            ?? user.FindFirst("name")?.Value
            ?? user.FindFirst(ClaimTypes.Name)?.Value 
            ?? string.Empty;
    }

    public static string GetEmail(this ClaimsPrincipal user)
    {
        return user.FindFirst("email")?.Value 
            ?? user.FindFirst(ClaimTypes.Email)?.Value 
            ?? string.Empty;
    }

    public static SubscriptionTier GetSubscriptionTier(this ClaimsPrincipal user)
    {
        var tierClaim = user.FindFirst("subscription_tier")?.Value;

        if (string.IsNullOrEmpty(tierClaim))
            return SubscriptionTier.Free;

        // Parsa som enum namn (ex: "Silver", "Gold")
        if (Enum.TryParse<SubscriptionTier>(tierClaim, ignoreCase: true, out var tier))
            return tier;

        // Fallback: parsa som nummer
        if (int.TryParse(tierClaim, out var tierNum))
            return (SubscriptionTier)tierNum;

        return SubscriptionTier.Free;
    }

    public static Role GetRole(this ClaimsPrincipal user)
    {
        var roleClaim = user.FindFirst("role")?.Value
            ?? user.FindFirst("userRole")?.Value;

        if (string.IsNullOrEmpty(roleClaim))
            return Role.User;

        if (Enum.TryParse<Role>(roleClaim, ignoreCase: true, out var role))
            return role;

        // Fallback: parsa som nummer
        if (int.TryParse(roleClaim, out var roleNum))
            return (Role)roleNum;

        return Role.User;

    }

    public static bool IsEmailConfirmed(this ClaimsPrincipal user)
    {
        var confirmedClaim = user.FindFirst("email_confirmed")?.Value 
            ?? user.FindFirst("emailVerified")?.Value
            ?? user.FindFirst("emailConfirmed")?.Value;
        return bool.TryParse(confirmedClaim, out var confirmed) && confirmed;
    }
}
