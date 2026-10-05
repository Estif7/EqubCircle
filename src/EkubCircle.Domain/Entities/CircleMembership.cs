using EkubCircle.Domain.Enums;

namespace EkubCircle.Domain.Entities;

public class CircleMembership
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CircleId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public CircleRole RoleInCircle { get; set; } = CircleRole.MEMBER;
    public int? PayoutOrder { get; set; }
    public MembershipStatus Status { get; set; } = MembershipStatus.ACTIVE;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Circle Circle { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}
