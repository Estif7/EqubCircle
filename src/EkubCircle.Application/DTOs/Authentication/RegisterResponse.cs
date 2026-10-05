namespace EkubCircle.Application.DTOs.Authentication;

public class RegisterResponse
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? DevSimulatedOtp { get; set; }
}
