using EkubCircle.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EkubCircle.Infrastructure.Persistence.Configurations;

public class PayoutConfiguration : IEntityTypeConfiguration<Payout>
{
    public void Configure(EntityTypeBuilder<Payout> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.RoundId)
            .IsRequired();

        builder.Property(p => p.ReceiverMembershipId)
            .IsRequired();

        builder.Property(p => p.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.PaidAt)
            .IsRequired();

        builder.Property(p => p.ConfirmedByUserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        // A round can have only one successful payout
        builder.HasIndex(p => p.RoundId)
            .IsUnique();

        builder.HasIndex(p => p.ReceiverMembershipId);

        builder.HasOne(p => p.Round)
            .WithMany()
            .HasForeignKey(p => p.RoundId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.ReceiverMembership)
            .WithMany()
            .HasForeignKey(p => p.ReceiverMembershipId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.ConfirmedByUser)
            .WithMany()
            .HasForeignKey(p => p.ConfirmedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
