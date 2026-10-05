namespace EkubCircle.Application.DTOs.Authentication;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool FaydaVerified { get; set; }
    public DateTime CreatedAt { get; set; }
}
