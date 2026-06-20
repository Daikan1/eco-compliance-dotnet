using System.ComponentModel.DataAnnotations;

namespace EcoCompliance.API.ViewModels.EsgIndicator;

public class EsgIndicatorRequest
{
    [Required(ErrorMessage = "CompanyId is required")]
    [Range(1, long.MaxValue, ErrorMessage = "CompanyId must be greater than 0")]
    public long CompanyId { get; set; }

    [Required(ErrorMessage = "IndicatorType is required")]
    [MaxLength(100)]
    public string IndicatorType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Value is required")]
    public double Value { get; set; }

    [MaxLength(50)]
    public string? Unit { get; set; }

    [Required(ErrorMessage = "MeasuredAt is required")]
    public DateTime MeasuredAt { get; set; }
}
