namespace EkubCircle.Application.DTOs.Payouts;

public class PayoutDto
{
    public Guid Id { get; set; }
    public Guid RoundId { get; set; }
    public int RoundNumber { get; set; }
    public Guid CircleId { get; set; }
    public string CircleName { get; set; } = string.Empty;
    public Guid ReceiverMembershipId { get; set; }
    public string ReceiverUserId { get; set; } = string.Empty;
    public string ReceiverName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; }
    public string ConfirmedByUserId { get; set; } = string.Empty;
    public string ConfirmedByName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
