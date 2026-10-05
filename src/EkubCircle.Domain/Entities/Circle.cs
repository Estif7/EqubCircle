using EkubCircle.Domain.Enums;

namespace EkubCircle.Domain.Entities;

public class Circle
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public decimal ContributionAmount { get; set; }
    public CircleFrequency Frequency { get; set; } = CircleFrequency.MONTHLY;
    public int MemberLimit { get; set; }
    public CircleStatus Status { get; set; } = CircleStatus.OPEN;
    public string OrganizerId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    public ApplicationUser Organizer { get; set; } = null!;
    public ICollection<CircleMembership> Memberships { get; set; } = new List<CircleMembership>();
}
