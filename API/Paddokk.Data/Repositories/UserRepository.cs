using Microsoft.EntityFrameworkCore;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Repositories;

public class UserRepository(PaddokkDbContext db) : IUserRepository
{
    private readonly PaddokkDbContext _db = db;

    public async Task<ApplicationUser?> GetByIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await _db.Users
            .Include(u => u.Cars)
            .Include(u => u.Journeys)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _db.Users
            .Include(u => u.Cars)
            .Include(u => u.Journeys)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<ApplicationUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return await _db.Users
            .Include(u => u.Cars)
            .Include(u => u.Journeys)
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken)
    {
        return await _db.Users.AnyAsync(u => u.Username == username, cancellationToken);
    }

    public async Task CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task SoftDeleteAsync(string userId, CancellationToken cancellationToken)
    {
        await _db.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(u => u
            .SetProperty(p => p.IsDeleted, true)
            .SetProperty(p => p.DeletedAt, DateTime.UtcNow), cancellationToken);
    }
}
