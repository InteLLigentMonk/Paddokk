using Paddokk.Core.Models;

namespace Paddokk.Core.Common;

public static class Subscriptions
{
    public static async Task<Result> SubscribeAsync<TSubject, TRelation>(
        SubjectLookup<TSubject> subject,
        SubscriptionOps<TRelation> relation,
        Func<TRelation> newRelation,
        string actorUserId,
        CancellationToken cancellationToken)
        where TSubject : class
        where TRelation : class, IActivatable
    {
        var loaded = await subject.LoadAsync(cancellationToken);

        if (loaded is null)
            return Result.Failure(Error.NotFound($"{subject.Label} not found"));

        if (subject.PrincipalIdOf(loaded) == actorUserId)
            return Result.Failure(Error.Conflict($"Cannot subscribe to your own {subject.Label.ToLowerInvariant()}"));

        var existing = await relation.FindAsync(cancellationToken);

        if (existing is not null)
        {
            if (!existing.IsActive)
            {
                existing.IsActive = true;
                await relation.UpdateAsync(existing, cancellationToken);
            }

            return Result.Success();
        }

        await relation.CreateAsync(newRelation(), cancellationToken);
        return Result.Success();
    }

    public static async Task<Result> UnsubscribeAsync<TRelation>(
        ToggleOps<TRelation> relation,
        CancellationToken cancellationToken)
        where TRelation : class, IActivatable
    {
        var existing = await relation.FindAsync(cancellationToken);

        if (existing is null)
            return Result.Success();

        existing.IsActive = false;
        await relation.UpdateAsync(existing, cancellationToken);
        return Result.Success();
    }
}

public sealed record SubjectLookup<T>(
    string Label,
    Func<CancellationToken, Task<T?>> LoadAsync,
    Func<T, string> PrincipalIdOf);

public sealed record ToggleOps<TRelation>(
    Func<CancellationToken, Task<TRelation?>> FindAsync,
    Func<TRelation, CancellationToken, Task> UpdateAsync);

public sealed record SubscriptionOps<TRelation>(
    Func<CancellationToken, Task<TRelation?>> FindAsync,
    Func<TRelation, CancellationToken, Task> CreateAsync,
    Func<TRelation, CancellationToken, Task> UpdateAsync)
{
    public ToggleOps<TRelation> ToToggleOps() => new(FindAsync, UpdateAsync);
}
