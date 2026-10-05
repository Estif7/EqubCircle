using EkubCircle.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace EkubCircle.Infrastructure.Persistence;

public class DatabaseSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        UserManager<ApplicationUser> userManager,
        ILogger<DatabaseSeeder> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await SeedUsersAsync();
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
}
