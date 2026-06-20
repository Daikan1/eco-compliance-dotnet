using System.ComponentModel.DataAnnotations;

namespace EcoCompliance.API.ViewModels.Compliance;

public class ComplianceRequest
{
    [Required(ErrorMessage = "CompanyId is required")]
    [Range(1, long.MaxValue, ErrorMessage = "CompanyId must be greater than 0")]
    public long CompanyId { get; set; }

    [Required(ErrorMessage = "ComplianceType is required")]
    [MaxLength(255)]
    public string ComplianceType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Status is required")]
    [MaxLength(100)]
    public string Status { get; set; } = string.Empty;

    [Required(ErrorMessage = "ReportDate is required")]
    public DateTime ReportDate { get; set; }
}
