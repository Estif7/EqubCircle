using EkubCircle.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EkubCircle.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.RoundId)
            .IsRequired();

        builder.Property(p => p.MembershipId)
            .IsRequired();

        builder.Property(p => p.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.PaidAt)
            .IsRequired();

        builder.Property(p => p.Reference)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Note)
            .HasMaxLength(500);

        builder.Property(p => p.RecordedByUserId)
            .IsRequired()
            .HasMaxLength(450);

        // Unique constraint: One member can have only one payment for a particular round
        builder.HasIndex(p => new { p.RoundId, p.MembershipId })
            .IsUnique();

        builder.HasOne(p => p.Round)
            .WithMany()
            .HasForeignKey(p => p.RoundId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Membership)
            .WithMany()
            .HasForeignKey(p => p.MembershipId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.RecordedByUser)
            .WithMany()
            .HasForeignKey(p => p.RecordedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
