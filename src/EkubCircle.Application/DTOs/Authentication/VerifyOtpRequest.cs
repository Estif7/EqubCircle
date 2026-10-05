using System.ComponentModel.DataAnnotations;

namespace EkubCircle.Application.DTOs.Authentication;

public class VerifyOtpRequest
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [StringLength(10, MinimumLength = 4)]
    public string OtpCode { get; set; } = string.Empty;
}
