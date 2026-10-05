using EkubCircle.Domain.Entities;
using EkubCircle.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EkubCircle.Infrastructure.Persistence;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await SeedUsersAsync();
        await SeedCirclesAsync();
    }

    private async Task SeedUsersAsync()
    {
        var demoUsers = new[]
        {
            new
            {
                Email = "abebe@ekubcircle.com",
                FullName = "Abebe Bikila",
                PhoneNumber = "+251911223344",
                Password = "Password123!"
            },
            new
            {
                Email = "almaz@ekubcircle.com",
                FullName = "Almaz Ayana",
                PhoneNumber = "+251922334455",
                Password = "Password123!"
            },
            new
            {
                Email = "haile@ekubcircle.com",
                FullName = "Haile Gebrselassie",
                PhoneNumber = "+251933445566",
                Password = "Password123!"
            },
            new
            {
                Email = "derartu@ekubcircle.com",
                FullName = "Derartu Tulu",
                PhoneNumber = "+251944556677",
                Password = "Password123!"
            }
        };

        foreach (var demoUser in demoUsers)
        {
            var existingUser = await _userManager.FindByEmailAsync(demoUser.Email);
            if (existingUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = demoUser.Email,
                    Email = demoUser.Email,
                    PhoneNumber = demoUser.PhoneNumber,
                    FullName = demoUser.FullName,
                    FaydaVerified = true,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, demoUser.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Seeded demo user: {Email}", demoUser.Email);
                }
                else
                {
                    _logger.LogWarning("Failed to seed user {Email}: {Errors}",
                        demoUser.Email,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    private async Task SeedCirclesAsync()
    {
        var abebe = await _userManager.FindByEmailAsync("abebe@ekubcircle.com");
        var almaz = await _userManager.FindByEmailAsync("almaz@ekubcircle.com");
        var haile = await _userManager.FindByEmailAsync("haile@ekubcircle.com");
        var derartu = await _userManager.FindByEmailAsync("derartu@ekubcircle.com");

        if (abebe == null || almaz == null || haile == null || derartu == null)
        {
            return;
        }

        // Circle 1: Addis Tech Savings Circle
        if (!await _context.Circles.AnyAsync(c => c.Name == "Addis Tech Savings Circle"))
        {
            var circle1 = new Circle
            {
                Name = "Addis Tech Savings Circle",
                ContributionAmount = 5000.00m,
                Frequency = CircleFrequency.MONTHLY,
                MemberLimit = 5,
                Status = CircleStatus.OPEN,
                OrganizerId = abebe.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            };

            circle1.Memberships.Add(new CircleMembership
            {
                CircleId = circle1.Id,
                UserId = abebe.Id,
                RoleInCircle = CircleRole.ORGANIZER,
                Status = MembershipStatus.ACTIVE,
                JoinedAt = DateTime.UtcNow.AddDays(-5)
            });

            circle1.Memberships.Add(new CircleMembership
            {
                CircleId = circle1.Id,
                UserId = haile.Id,
                RoleInCircle = CircleRole.MEMBER,
                Status = MembershipStatus.ACTIVE,
                JoinedAt = DateTime.UtcNow.AddDays(-4)
            });

            _context.Circles.Add(circle1);
            _logger.LogInformation("Seeded demo circle: {Name}", circle1.Name);
        }

        // Circle 2: Bole Entrepreneurs Ekub
        if (!await _context.Circles.AnyAsync(c => c.Name == "Bole Entrepreneurs Ekub"))
        {
            var circle2 = new Circle
            {
                Name = "Bole Entrepreneurs Ekub",
                ContributionAmount = 10000.00m,
                Frequency = CircleFrequency.MONTHLY,
                MemberLimit = 4,
                Status = CircleStatus.OPEN,
                OrganizerId = almaz.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            };

            circle2.Memberships.Add(new CircleMembership
            {
                CircleId = circle2.Id,
                UserId = almaz.Id,
                RoleInCircle = CircleRole.ORGANIZER,
                Status = MembershipStatus.ACTIVE,
                JoinedAt = DateTime.UtcNow.AddDays(-3)
            });

            circle2.Memberships.Add(new CircleMembership
            {
                CircleId = circle2.Id,
                UserId = derartu.Id,
                RoleInCircle = CircleRole.MEMBER,
                Status = MembershipStatus.ACTIVE,
                JoinedAt = DateTime.UtcNow.AddDays(-2)
            });

            _context.Circles.Add(circle2);
            _logger.LogInformation("Seeded demo circle: {Name}", circle2.Name);
        }

        await _context.SaveChangesAsync();
    }
}
