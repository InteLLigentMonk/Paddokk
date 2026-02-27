namespace Paddokk.Core.Interfaces;

/// <summary>
/// Provides access to the current authenticated user within the service layer.
/// </summary>
public interface IActorResolver
{
    string UserId { get; }
    bool IsAuthenticated { get; }
}