namespace EkubCircle.Application.DTOs.Authentication;

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public UserDto User { get; set; } = null!;
}
