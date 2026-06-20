namespace EcoCompliance.API.ViewModels.Company;

public class CompanyResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Cnpj { get; set; }
    public string? Sector { get; set; }
}
