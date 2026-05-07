using MediatR;
using Paddokk.Core.Interfaces;

namespace Paddokk.Core.Features.Users.Commands.EnsureUserExists;

/// <summary>
/// Idempotent: creates an ApplicationUser if one does not already exist for the given UserId.
/// Called from the auth sync middleware on every authenticated request.
/// </summary>
public record EnsureUserExistsCommand(
    string UserId,
    string? Email,
    string? FullName,
    string? GivenName,
    string? FamilyName
) : ICommand<Unit>;
