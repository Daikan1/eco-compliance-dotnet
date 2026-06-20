using EcoCompliance.API.Exceptions;
using EcoCompliance.API.Models;
using EcoCompliance.API.Repositories.Interfaces;
using EcoCompliance.API.Services.Interfaces;
using EcoCompliance.API.ViewModels;
using EcoCompliance.API.ViewModels.Company;

namespace EcoCompliance.API.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _repository;

    public CompanyService(ICompanyRepository repository) => _repository = repository;

    public async Task<PagedResult<CompanyResponse>> GetAllAsync(int page, int pageSize)
    {
        var (items, total) = await _repository.GetAllAsync(page, pageSize);
        return new PagedResult<CompanyResponse>
        {
            Items = items.Select(ToResponse),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<CompanyResponse> GetByIdAsync(long id)
    {
        var company = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Company), id);
        return ToResponse(company);
    }

    public async Task<CompanyResponse> CreateAsync(CompanyRequest request)
    {
        var company = new Company
        {
            Name = request.Name,
            Cnpj = request.Cnpj,
            Sector = request.Sector
        };
        var created = await _repository.AddAsync(company);
        return ToResponse(created);
    }

    private static CompanyResponse ToResponse(Company c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Cnpj = c.Cnpj,
        Sector = c.Sector
    };
}
