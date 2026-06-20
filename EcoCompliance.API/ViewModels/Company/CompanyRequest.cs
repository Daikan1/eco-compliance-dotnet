using System.ComponentModel.DataAnnotations;

namespace EcoCompliance.API.ViewModels.Company;

public class CompanyRequest
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Cnpj { get; set; }

    [MaxLength(100)]
    public string? Sector { get; set; }
}
