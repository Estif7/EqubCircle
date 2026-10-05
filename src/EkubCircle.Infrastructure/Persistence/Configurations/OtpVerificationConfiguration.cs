using EkubCircle.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EkubCircle.Infrastructure.Persistence.Configurations;

public class OtpVerificationConfiguration : IEntityTypeConfiguration<OtpVerification>
{
    public void Configure(EntityTypeBuilder<OtpVerification> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(o => o.PhoneNumber)
            .IsRequired()
            .HasMaxLength(25);

        builder.Property(o => o.OtpHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(o => o.ExpiresAt)
            .IsRequired();

        builder.Property(o => o.Attempts)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.HasIndex(o => o.UserId);
    }
}
