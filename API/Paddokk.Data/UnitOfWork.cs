using Paddokk.Core.Interfaces;

namespace Paddokk.Data;

public class UnitOfWork(PaddokkDbContext db) : IUnitOfWork
{
    private readonly PaddokkDbContext _db = db;

    public async Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await operation();
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw; // Let GlobalExceptionMiddleware handle it
        }
    }
}