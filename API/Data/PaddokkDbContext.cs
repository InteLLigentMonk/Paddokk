using System.Linq.Expressions;
using API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

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

        // Configure Identity tables to use shorter names
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(e => e.DisplayName).HasMaxLength(100);
            entity.Property(e => e.Bio).HasMaxLength(500);

            // Self-referencing relationship for default active journey
            entity.HasOne(e => e.DefaultActiveJourney)
                .WithMany()
                .HasForeignKey(e => e.DefaultActiveJourneyId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Car database configuration
        modelBuilder.Entity<CarMake>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
        });

        modelBuilder.Entity<CarModel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.CarMakeId, e.Name }).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(e => e.CarMake)
                .WithMany(e => e.Models)
                .HasForeignKey(e => e.CarMakeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CarGeneration>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(e => e.CarModel)
                .WithMany(e => e.Generations)
                .HasForeignKey(e => e.CarModelId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // UserCar configuration
        modelBuilder.Entity<UserCar>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nickname).HasMaxLength(100);
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(1000);

            entity.HasOne(e => e.User)
                .WithMany(e => e.Cars)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CarMake)
                .WithMany()
                .HasForeignKey(e => e.CarMakeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CarModel)
                .WithMany()
                .HasForeignKey(e => e.CarModelId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CarGeneration)
                .WithMany()
                .HasForeignKey(e => e.CarGenerationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Journey configuration
        modelBuilder.Entity<Journey>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);

            entity.HasOne(e => e.User)
                .WithMany(e => e.Journeys)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.UserCar)
                .WithMany(e => e.Journeys)
                .HasForeignKey(e => e.UserCarId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // JourneyPost configuration
        modelBuilder.Entity<JourneyPost>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TextContent).HasMaxLength(5000);

            entity.HasOne(e => e.Journey)
                .WithMany(e => e.Posts)
                .HasForeignKey(e => e.JourneyId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // JourneyPostImage configuration
        modelBuilder.Entity<JourneyPostImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Caption).HasMaxLength(500);

            entity.HasOne(e => e.JourneyPost)
                .WithMany(e => e.Images)
                .HasForeignKey(e => e.JourneyPostId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // JourneySubscription configuration
        modelBuilder.Entity<JourneySubscription>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.JourneyId }).IsUnique();

            entity.HasOne(e => e.User)
                .WithMany(e => e.JourneySubscriptions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Journey)
                .WithMany(e => e.Subscriptions)
                .HasForeignKey(e => e.JourneyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // JourneyLike configuration
        modelBuilder.Entity<JourneyLike>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.JourneyId }).IsUnique();

            entity.HasOne(e => e.User)
                .WithMany(e => e.JourneyLikes)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Journey)
                .WithMany(e => e.Likes)
                .HasForeignKey(e => e.JourneyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // PostComment configuration
        modelBuilder.Entity<PostComment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).HasMaxLength(2000);

            entity.HasOne(e => e.JourneyPost)
                .WithMany(e => e.Comments)
                .HasForeignKey(e => e.JourneyPostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany(e => e.Comments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<UserCarImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.ThumbnailUrl).HasMaxLength(500);
            entity.Property(e => e.MediumUrl).HasMaxLength(500);
            entity.Property(e => e.Caption).HasMaxLength(500);
            entity.Property(e => e.ContentType).HasMaxLength(100);

            entity.HasOne(e => e.UserCar)
                .WithMany(e => e.Images)
                .HasForeignKey(e => e.UserCarId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ensure only one primary image per car
            entity.HasIndex(e => new { e.UserCarId, e.IsPrimary })
                .HasFilter("[IsPrimary] = 1")
                .IsUnique();
        });

        // Seed data
        SeedData(modelBuilder);
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

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed car makes
        modelBuilder.Entity<CarMake>().HasData(
            new CarMake { Id = 1, Name = "Toyota", Country = "Japan", Group = CarGroup.JDM },
            new CarMake { Id = 2, Name = "Honda", Country = "Japan", Group = CarGroup.JDM },
            new CarMake { Id = 3, Name = "Nissan", Country = "Japan", Group = CarGroup.JDM },
            new CarMake { Id = 4, Name = "Mazda", Country = "Japan", Group = CarGroup.JDM },
            new CarMake { Id = 5, Name = "Subaru", Country = "Japan", Group = CarGroup.JDM },
            new CarMake { Id = 6, Name = "BMW", Country = "Germany", Group = CarGroup.German },
            new CarMake { Id = 7, Name = "Mercedes-Benz", Country = "Germany", Group = CarGroup.German },
            new CarMake { Id = 8, Name = "Audi", Country = "Germany", Group = CarGroup.German },
            new CarMake { Id = 9, Name = "Volkswagen", Country = "Germany", Group = CarGroup.German },
            new CarMake { Id = 10, Name = "Porsche", Country = "Germany", Group = CarGroup.German },
            new CarMake { Id = 11, Name = "Ford", Country = "USA", Group = CarGroup.American },
            new CarMake { Id = 12, Name = "Chevrolet", Country = "USA", Group = CarGroup.American },
            new CarMake { Id = 13, Name = "Dodge", Country = "USA", Group = CarGroup.American }
        );

        // Seed popular car models
        modelBuilder.Entity<CarModel>().HasData(
            // Toyota
            new CarModel { Id = 1, Name = "Supra", CarMakeId = 1 },
            new CarModel { Id = 2, Name = "Corolla", CarMakeId = 1 },
            new CarModel { Id = 3, Name = "86", CarMakeId = 1 },
            new CarModel { Id = 4, Name = "MR2", CarMakeId = 1 },

            // Honda
            new CarModel { Id = 5, Name = "Civic", CarMakeId = 2 },
            new CarModel { Id = 6, Name = "S2000", CarMakeId = 2 },
            new CarModel { Id = 7, Name = "NSX", CarMakeId = 2 },
            new CarModel { Id = 8, Name = "Integra", CarMakeId = 2 },

            // Nissan
            new CarModel { Id = 9, Name = "240SX", CarMakeId = 3 },
            new CarModel { Id = 10, Name = "GT-R", CarMakeId = 3 },
            new CarModel { Id = 11, Name = "350Z", CarMakeId = 3 },
            new CarModel { Id = 12, Name = "370Z", CarMakeId = 3 },

            // BMW
            new CarModel { Id = 13, Name = "M3", CarMakeId = 6 },
            new CarModel { Id = 14, Name = "M4", CarMakeId = 6 },
            new CarModel { Id = 15, Name = "335i", CarMakeId = 6 },

            // Ford
            new CarModel { Id = 16, Name = "Mustang", CarMakeId = 11 },
            new CarModel { Id = 17, Name = "Focus", CarMakeId = 11 }
        );

        // Seed some car generations
        modelBuilder.Entity<CarGeneration>().HasData(
            // Toyota Supra
            new CarGeneration { Id = 1, Name = "A90", StartYear = 2019, CarModelId = 1 },
            new CarGeneration { Id = 2, Name = "A80", StartYear = 1993, EndYear = 2002, CarModelId = 1 },

            // Honda Civic
            new CarGeneration { Id = 3, Name = "EK", StartYear = 1996, EndYear = 2000, CarModelId = 5 },
            new CarGeneration { Id = 4, Name = "EP3", StartYear = 2001, EndYear = 2005, CarModelId = 5 },

            // Nissan 240SX
            new CarGeneration { Id = 5, Name = "S13", StartYear = 1989, EndYear = 1994, CarModelId = 9 },
            new CarGeneration { Id = 6, Name = "S14", StartYear = 1995, EndYear = 1998, CarModelId = 9 },

            // BMW M3
            new CarGeneration { Id = 7, Name = "E46", StartYear = 2000, EndYear = 2006, CarModelId = 13 },
            new CarGeneration { Id = 8, Name = "E92", StartYear = 2007, EndYear = 2013, CarModelId = 13 }
        );
    }
}
