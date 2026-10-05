using EkubCircle.Application.DTOs.Circles;

namespace EkubCircle.Application.DTOs.Dashboard;

public class UserDashboardDto
{
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int OrganizedCirclesCount { get; set; }
    public int JoinedCirclesCount { get; set; }
    public decimal TotalContributed { get; set; }
    public decimal TotalReceived { get; set; }
    public int PendingPaymentsCount { get; set; }
    public List<CircleSummaryDto> ActiveCircles { get; set; } = new();
    public List<UserPendingPaymentDto> PendingPayments { get; set; } = new();
}

public class UserPendingPaymentDto
{
    public Guid CircleId { get; set; }
    public string CircleName { get; set; } = string.Empty;
    public Guid RoundId { get; set; }
    public int RoundNumber { get; set; }
    public Guid MembershipId { get; set; }
    public decimal AmountDue { get; set; }
}
