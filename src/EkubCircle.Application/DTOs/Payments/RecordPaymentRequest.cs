using System.ComponentModel.DataAnnotations;

namespace EkubCircle.Application.DTOs.Payments;

public class RecordPaymentRequest
{
    [Required]
    public Guid MembershipId { get; set; }

    [Required]
    [Range(1, 100000000)]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Reference { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Note { get; set; }
}
