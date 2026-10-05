namespace EkubCircle.Application.DTOs.Payments;

public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid RoundId { get; set; }
    public int RoundNumber { get; set; }
    public Guid MembershipId { get; set; }
    public string MemberUserId { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public string MemberEmail { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string RecordedByUserId { get; set; } = string.Empty;
    public string RecordedByName { get; set; } = string.Empty;
}
