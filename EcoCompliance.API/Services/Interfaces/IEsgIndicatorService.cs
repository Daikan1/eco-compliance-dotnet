using EcoCompliance.API.ViewModels;
using EcoCompliance.API.ViewModels.EsgIndicator;

namespace EcoCompliance.API.Services.Interfaces;

public interface IEsgIndicatorService
{
    Task<PagedResult<EsgIndicatorResponse>> GetAllAsync(int page, int pageSize);
    Task<EsgIndicatorResponse> CreateAsync(EsgIndicatorRequest request);
}
