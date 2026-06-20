using EcoCompliance.API.Models;

namespace EcoCompliance.API.Repositories.Interfaces;

public interface IComplianceRepository
{
    Task<(IEnumerable<ComplianceRecord> Items, int TotalCount)> GetAllAsync(int page, int pageSize);
    Task<(IEnumerable<ComplianceRecord> Items, int TotalCount)> GetByCompanyAsync(long companyId, int page, int pageSize);
    Task<ComplianceRecord?> GetByIdAsync(long id);
    Task<ComplianceRecord> AddAsync(ComplianceRecord record);
    Task<ComplianceRecord> UpdateAsync(ComplianceRecord record);
    Task DeleteAsync(ComplianceRecord record);
}
