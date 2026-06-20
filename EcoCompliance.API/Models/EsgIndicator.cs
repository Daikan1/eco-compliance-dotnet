namespace EcoCompliance.API.Models;

public class EsgIndicator
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    public string IndicatorType { get; set; } = string.Empty;
    public double Value { get; set; }
    public string? Unit { get; set; }
    public DateTime MeasuredAt { get; set; }
}
