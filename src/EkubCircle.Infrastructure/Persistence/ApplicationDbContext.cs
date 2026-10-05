using EkubCircle.Application.Common.Interfaces;
using EkubCircle.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EkubCircle.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<OtpVerification> OtpVerifications => Set<OtpVerification>();
    public DbSet<Circle> Circles => Set<Circle>();
    public DbSet<CircleMembership> CircleMemberships => Set<CircleMembership>();
    public DbSet<Round> Rounds => Set<Round>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
