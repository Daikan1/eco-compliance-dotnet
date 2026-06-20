using EcoCompliance.API.ViewModels;
using EcoCompliance.API.ViewModels.Company;

namespace EcoCompliance.API.Services.Interfaces;

public interface ICompanyService
{
    Task<PagedResult<CompanyResponse>> GetAllAsync(int page, int pageSize);
    Task<CompanyResponse> GetByIdAsync(long id);
    Task<CompanyResponse> CreateAsync(CompanyRequest request);
}
