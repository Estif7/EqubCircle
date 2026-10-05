using Microsoft.AspNetCore.Identity;

namespace EkubCircle.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public bool FaydaVerified { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
