using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data;

public class PaddokkDbContext : DbContext
{
    public PaddokkDbContext(DbContextOptions<PaddokkDbContext> options) : base(options)
    {
    }

    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<UserCar> UserCars { get; set; }
    public DbSet<CarMake> CarMakes { get; set; }
    public DbSet<CarModel> CarModels { get; set; }
    public DbSet<CarGeneration> CarGenerations { get; set; }
    public DbSet<Journey> Journeys { get; set; }
    public DbSet<JourneyPost> JourneyPosts { get; set; }
    public DbSet<JourneyPostImage> JourneyPostImages { get; set; }
    public DbSet<JourneySubscription> JourneySubscriptions { get; set; }
    public DbSet<JourneyLike> JourneyLikes { get; set; }
    public DbSet<PostComment> PostComments { get; set; }
    public DbSet<UserCarImage> UserCarImages { get; set; }
    public DbSet<UserCarLike> UserCarLikes { get; set; }
    public DbSet<UserCarSubscription> UserCarSubscriptions { get; set; }
    public DbSet<ReservedUsername> ReservedUsernames { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Filtrera bort raderade användare
        modelBuilder.Entity<ApplicationUser>()
            .HasQueryFilter(u => !u.IsDeleted);

        // Hitta alla entiteter med UserId foreign key
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Kolla om entiteten har en User navigation property
            var userNavigation = entityType.GetNavigations()
                .FirstOrDefault(n => n.ClrType == typeof(ApplicationUser));

            if (userNavigation != null)
            {
                // Skapa filter: entity => !entity.User.IsDeleted
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var userProperty = Expression.Property(parameter, userNavigation.Name);
                var isDeletedProperty = Expression.Property(userProperty, nameof(ApplicationUser.IsDeleted));
                var notDeleted = Expression.Not(isDeletedProperty);
                var lambda = Expression.Lambda(notDeleted, parameter);

                entityType.SetQueryFilter(lambda);
            }
        }

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaddokkDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<ApplicationUser>()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            entry.Entity.UpdatedAt = DateTime.UtcNow;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
