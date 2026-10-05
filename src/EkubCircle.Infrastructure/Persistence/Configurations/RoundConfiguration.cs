using EkubCircle.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EkubCircle.Infrastructure.Persistence.Configurations;

public class RoundConfiguration : IEntityTypeConfiguration<Round>
{
    public void Configure(EntityTypeBuilder<Round> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.CircleId)
            .IsRequired();

        builder.Property(r => r.RoundNumber)
            .IsRequired();

        builder.Property(r => r.ReceiverMembershipId)
            .IsRequired();

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(r => r.PayoutAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        // Unique constraint: A round number must be unique per circle
        builder.HasIndex(r => new { r.CircleId, r.RoundNumber })
            .IsUnique();

        builder.HasOne(r => r.Circle)
            .WithMany()
            .HasForeignKey(r => r.CircleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.ReceiverMembership)
            .WithMany()
            .HasForeignKey(r => r.ReceiverMembershipId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
