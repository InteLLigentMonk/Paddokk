using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Configurations;

public class PostCommentConfiguration : IEntityTypeConfiguration<PostComment>
{
    public void Configure(EntityTypeBuilder<PostComment> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Content).HasMaxLength(500);

        builder.HasOne(e => e.JourneyPost)
            .WithMany(e => e.Comments)
            .HasForeignKey(e => e.JourneyPostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Author)
            .WithMany(e => e.Comments)
            .HasForeignKey(e => e.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ParentComment)
            .WithMany(e => e.Replies)
            .HasForeignKey(e => e.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
