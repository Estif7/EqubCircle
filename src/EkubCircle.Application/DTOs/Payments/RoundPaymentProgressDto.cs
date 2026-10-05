using EkubCircle.Application.DTOs.Circles;

namespace EkubCircle.Application.DTOs.Payments;

public class RoundPaymentProgressDto
{
    public Guid RoundId { get; set; }
    public int RoundNumber { get; set; }
    public Guid CircleId { get; set; }
    public string CircleName { get; set; } = string.Empty;
    public decimal ContributionAmount { get; set; }
    public decimal ExpectedTotalAmount { get; set; }
    public decimal CollectedAmount { get; set; }
    public int TotalMembers { get; set; }
    public int PaidCount { get; set; }
    public int UnpaidCount { get; set; }
    public bool IsFullyPaid { get; set; }
    public List<CircleMemberDto> PaidMembers { get; set; } = new();
    public List<CircleMemberDto> UnpaidMembers { get; set; } = new();
}
