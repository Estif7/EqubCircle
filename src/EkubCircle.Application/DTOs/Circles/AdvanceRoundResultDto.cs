using EkubCircle.Application.DTOs.Rounds;

namespace EkubCircle.Application.DTOs.Circles;

public class AdvanceRoundResultDto
{
    public Guid CircleId { get; set; }
    public string CircleName { get; set; } = string.Empty;
    public string CircleStatus { get; set; } = string.Empty;
    public bool IsCircleCompleted { get; set; }
    public DateTime? CircleCompletedAt { get; set; }
    public RoundDto? PreviousRound { get; set; }
    public RoundDto? CurrentRound { get; set; }
    public string Message { get; set; } = string.Empty;
}
