namespace EcoCompliance.API.Models;

public class Company
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Cnpj { get; set; }
    public string? Sector { get; set; }

    public ICollection<ComplianceRecord> ComplianceRecords { get; set; } = new List<ComplianceRecord>();
    public ICollection<EsgIndicator> EsgIndicators { get; set; } = new List<EsgIndicator>();
}
