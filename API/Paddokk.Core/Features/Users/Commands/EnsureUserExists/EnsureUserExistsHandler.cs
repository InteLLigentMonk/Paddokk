using MediatR;
using Paddokk.Core.Common;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Users.Commands.EnsureUserExists;

public sealed class EnsureUserExistsHandler(
    IUserRepository userRepository,
    UsernameGenerator usernameGenerator)
    : IRequestHandler<EnsureUserExistsCommand, Unit>
{
    public async Task<Unit> Handle(EnsureUserExistsCommand request, CancellationToken cancellationToken)
    {
        var existing = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (existing is not null)
            return Unit.Value;

        var (firstName, lastName) = ResolveNames(request);
        var displayName = string.IsNullOrWhiteSpace(lastName) ? firstName : $"{firstName} {lastName}";

        var usernameCandidate = usernameGenerator.Generate(firstName, lastName);
        var username = await usernameGenerator.EnsureUniqueAsync(usernameCandidate, userRepository, cancellationToken);

        var newUser = new ApplicationUser
        {
            Id = request.UserId,
            FirstName = firstName,
            LastName = string.IsNullOrWhiteSpace(lastName) ? null : lastName,
            Username = username,
            DisplayName = displayName,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await userRepository.CreateAsync(newUser, cancellationToken);
        return Unit.Value;
    }

    private static (string FirstName, string? LastName) ResolveNames(EnsureUserExistsCommand request)
    {
        // Prefer explicit given/family name from social-provider claims
        if (!string.IsNullOrWhiteSpace(request.GivenName))
            return (request.GivenName.Trim(), request.FamilyName?.Trim());

        // Fall back to splitting the FullName on the first space
        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            var trimmed = request.FullName.Trim();
            var firstSpace = trimmed.IndexOf(' ');
            if (firstSpace < 0)
                return (trimmed, null);

            var first = trimmed[..firstSpace];
            var rest = trimmed[(firstSpace + 1)..].Trim();
            return (first, string.IsNullOrWhiteSpace(rest) ? null : rest);
        }

        // Last-resort fallback: email prefix (covers social logins that omit name entirely)
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var prefix = request.Email.Split('@')[0];
            return (prefix, null);
        }

        throw new InvalidOperationException(
            $"Cannot resolve names for user '{request.UserId}' — no name claims and no email available");
    }
}
