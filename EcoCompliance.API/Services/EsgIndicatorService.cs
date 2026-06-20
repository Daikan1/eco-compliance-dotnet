using EcoCompliance.API.Exceptions;
using EcoCompliance.API.Models;
using EcoCompliance.API.Repositories.Interfaces;
using EcoCompliance.API.Services.Interfaces;
using EcoCompliance.API.ViewModels;
using EcoCompliance.API.ViewModels.EsgIndicator;

namespace EcoCompliance.API.Services;

public class EsgIndicatorService : IEsgIndicatorService
{
    private readonly IEsgIndicatorRepository _repository;
    private readonly ICompanyRepository _companyRepository;

    public EsgIndicatorService(IEsgIndicatorRepository repository, ICompanyRepository companyRepository)
    {
        _repository = repository;
        _companyRepository = companyRepository;
    }

    public async Task<PagedResult<EsgIndicatorResponse>> GetAllAsync(int page, int pageSize)
    {
        var (items, total) = await _repository.GetAllAsync(page, pageSize);
        return new PagedResult<EsgIndicatorResponse>
        {
            Items = items.Select(ToResponse),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<EsgIndicatorResponse> CreateAsync(EsgIndicatorRequest request)
    {
        _ = await _companyRepository.GetByIdAsync(request.CompanyId)
            ?? throw new NotFoundException(nameof(Company), request.CompanyId);

        var indicator = new EsgIndicator
        {
            CompanyId = request.CompanyId,
            IndicatorType = request.IndicatorType,
            Value = request.Value,
            Unit = request.Unit,
            MeasuredAt = request.MeasuredAt
        };

        var created = await _repository.AddAsync(indicator);
        return ToResponse(created);
    }

    private static EsgIndicatorResponse ToResponse(EsgIndicator i) => new()
    {
        Id = i.Id,
        CompanyId = i.CompanyId,
        CompanyName = i.Company?.Name ?? string.Empty,
        IndicatorType = i.IndicatorType,
        Value = i.Value,
        Unit = i.Unit,
        MeasuredAt = i.MeasuredAt
    };
}
