using EkubCircle.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EkubCircle.Infrastructure.Persistence.Configurations;

public class CircleMembershipConfiguration : IEntityTypeConfiguration<CircleMembership>
{
    public void Configure(EntityTypeBuilder<CircleMembership> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.CircleId)
            .IsRequired();

        builder.Property(m => m.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(m => m.RoleInCircle)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(m => m.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(m => m.JoinedAt)
            .IsRequired();

        // Unique constraint: A user cannot join the same circle twice
        builder.HasIndex(m => new { m.CircleId, m.UserId })
            .IsUnique();

        builder.HasOne(m => m.Circle)
            .WithMany(c => c.Memberships)
            .HasForeignKey(m => m.CircleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
