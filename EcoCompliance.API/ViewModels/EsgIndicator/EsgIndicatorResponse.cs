namespace EcoCompliance.API.ViewModels.EsgIndicator;

public class EsgIndicatorResponse
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string IndicatorType { get; set; } = string.Empty;
    public double Value { get; set; }
    public string? Unit { get; set; }
    public DateTime MeasuredAt { get; set; }
}
