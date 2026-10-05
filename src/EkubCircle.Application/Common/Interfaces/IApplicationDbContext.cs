using EkubCircle.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EkubCircle.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<OtpVerification> OtpVerifications { get; }
    DbSet<Circle> Circles { get; }
    DbSet<CircleMembership> CircleMemberships { get; }
    DbSet<Round> Rounds { get; }
    DbSet<Payment> Payments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
