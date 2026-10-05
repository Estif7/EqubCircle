namespace EkubCircle.Application.DTOs.Rounds;

public class RoundDto
{
    public Guid Id { get; set; }
    public Guid CircleId { get; set; }
    public int RoundNumber { get; set; }
    public Guid ReceiverMembershipId { get; set; }
    public string ReceiverUserId { get; set; } = string.Empty;
    public string ReceiverName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal ExpectedPayoutAmount { get; set; }
    public decimal PayoutAmount { get; set; }
    public DateTime? OpenedAt { get; set; }
    public DateTime? PaidOutAt { get; set; }
    public bool IsCurrentRound { get; set; }
}
