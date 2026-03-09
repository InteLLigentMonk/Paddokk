using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Configurations;

public class UserCarSubscriptionConfiguration : IEntityTypeConfiguration<UserCarSubscription>
{
    public void Configure(EntityTypeBuilder<UserCarSubscription> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => new { e.UserId, e.UserCarId }).IsUnique();

        builder.HasOne(e => e.User)
            .WithMany(e => e.UserCarSubscriptions)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.UserCar)
            .WithMany(e => e.Subscriptions)
            .HasForeignKey(e => e.UserCarId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
