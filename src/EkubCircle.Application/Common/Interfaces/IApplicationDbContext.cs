using EkubCircle.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EkubCircle.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<OtpVerification> OtpVerifications { get; }
    DbSet<Circle> Circles { get; }
    DbSet<CircleMembership> CircleMemberships { get; }
    DbSet<Round> Rounds { get; }
    DbSet<Payment> Payments { get; }
    DbSet<Payout> Payouts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
