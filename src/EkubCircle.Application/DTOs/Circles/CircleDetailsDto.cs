namespace EkubCircle.Application.DTOs.Circles;

public class CircleDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal ContributionAmount { get; set; }
    public string Frequency { get; set; } = string.Empty;
    public int MemberLimit { get; set; }
    public int MemberCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string OrganizerId { get; set; } = string.Empty;
    public string OrganizerName { get; set; } = string.Empty;
    public bool IsOrganizer { get; set; }
    public bool IsMember { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<CircleMemberDto> Members { get; set; } = new();
}
