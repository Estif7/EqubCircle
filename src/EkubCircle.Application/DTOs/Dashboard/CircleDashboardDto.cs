using EkubCircle.Application.DTOs.Circles;
using EkubCircle.Application.DTOs.Payments;
using EkubCircle.Application.DTOs.Payouts;
using EkubCircle.Application.DTOs.Rounds;

namespace EkubCircle.Application.DTOs.Dashboard;

public class CircleDashboardDto
{
    public CircleDetailsDto Circle { get; set; } = null!;
    public RoundDto? ActiveRound { get; set; }
    public RoundPaymentProgressDto? ActiveRoundProgress { get; set; }
    public List<RoundDto> Rounds { get; set; } = new();
    public List<PayoutDto> Payouts { get; set; } = new();
    public int TotalRounds { get; set; }
    public int CompletedRounds { get; set; }
    public decimal TotalDisbursed { get; set; }
    public decimal TotalExpected { get; set; }
}
