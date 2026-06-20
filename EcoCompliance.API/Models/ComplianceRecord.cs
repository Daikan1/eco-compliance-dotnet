namespace EcoCompliance.API.Models;

public class ComplianceRecord
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    public string ComplianceType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; }
}
