using EcoCompliance.API.Models;

namespace EcoCompliance.API.Repositories.Interfaces;

public interface IEsgIndicatorRepository
{
    Task<(IEnumerable<EsgIndicator> Items, int TotalCount)> GetAllAsync(int page, int pageSize);
    Task<EsgIndicator> AddAsync(EsgIndicator indicator);
}
