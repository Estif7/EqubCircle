namespace EkubCircle.Application.DTOs.Circles;

public class CircleMemberDto
{
    public Guid MembershipId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string RoleInCircle { get; set; } = string.Empty;
    public int? PayoutOrder { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
}
