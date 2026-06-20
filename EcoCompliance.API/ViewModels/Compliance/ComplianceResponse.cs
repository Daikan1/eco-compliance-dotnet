namespace EcoCompliance.API.ViewModels.Compliance;

public class ComplianceResponse
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string ComplianceType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; }
}
