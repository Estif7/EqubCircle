using System.ComponentModel.DataAnnotations;
using EkubCircle.Domain.Enums;

namespace EkubCircle.Application.DTOs.Circles;

public class CreateCircleRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(10, 10000000)]
    public decimal ContributionAmount { get; set; }

    [Required]
    public CircleFrequency Frequency { get; set; } = CircleFrequency.MONTHLY;

    [Required]
    [Range(2, 500)]
    public int MemberLimit { get; set; }
}
