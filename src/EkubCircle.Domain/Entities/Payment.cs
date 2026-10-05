namespace EkubCircle.Domain.Entities;

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RoundId { get; set; }
    public Guid MembershipId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    public string Reference { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string RecordedByUserId { get; set; } = string.Empty;

    // Navigation properties
    public Round Round { get; set; } = null!;
    public CircleMembership Membership { get; set; } = null!;
    public ApplicationUser RecordedByUser { get; set; } = null!;
}
