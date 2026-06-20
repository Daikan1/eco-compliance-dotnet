using EcoCompliance.API.Models;

namespace EcoCompliance.API.Repositories.Interfaces;

public interface ICompanyRepository
{
    Task<(IEnumerable<Company> Items, int TotalCount)> GetAllAsync(int page, int pageSize);
    Task<Company?> GetByIdAsync(long id);
    Task<Company> AddAsync(Company company);
}
