using EcoCompliance.API.ViewModels;
using EcoCompliance.API.ViewModels.Compliance;

namespace EcoCompliance.API.Services.Interfaces;

public interface IComplianceService
{
    Task<PagedResult<ComplianceResponse>> GetAllAsync(int page, int pageSize);
    Task<PagedResult<ComplianceResponse>> GetByCompanyAsync(long companyId, int page, int pageSize);
    Task<ComplianceResponse> CreateAsync(ComplianceRequest request);
    Task<ComplianceResponse> UpdateAsync(long id, ComplianceRequest request);
    Task DeleteAsync(long id);
}
