using EkubCircle.Domain.Enums;

namespace EkubCircle.Domain.Entities;

public class Round
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CircleId { get; set; }
    public int RoundNumber { get; set; }
    public Guid ReceiverMembershipId { get; set; }
    public RoundStatus Status { get; set; } = RoundStatus.PENDING;
    public DateTime? OpenedAt { get; set; }
    public DateTime? PaidOutAt { get; set; }
    public decimal PayoutAmount { get; set; }

    // Navigation properties
    public Circle Circle { get; set; } = null!;
    public CircleMembership ReceiverMembership { get; set; } = null!;
}
