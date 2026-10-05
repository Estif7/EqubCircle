using EkubCircle.Domain.Enums;

namespace EkubCircle.Domain.Entities;

public class Payout
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RoundId { get; set; }
    public Guid ReceiverMembershipId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    public string ConfirmedByUserId { get; set; } = string.Empty;
    public PayoutStatus Status { get; set; } = PayoutStatus.COMPLETED;

    // Navigation properties
    public Round Round { get; set; } = null!;
    public CircleMembership ReceiverMembership { get; set; } = null!;
    public ApplicationUser ConfirmedByUser { get; set; } = null!;
}
