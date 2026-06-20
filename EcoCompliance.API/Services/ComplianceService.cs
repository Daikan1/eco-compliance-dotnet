using EcoCompliance.API.Exceptions;
using EcoCompliance.API.Models;
using EcoCompliance.API.Repositories.Interfaces;
using EcoCompliance.API.Services.Interfaces;
using EcoCompliance.API.ViewModels;
using EcoCompliance.API.ViewModels.Compliance;

namespace EcoCompliance.API.Services;

public class ComplianceService : IComplianceService
{
    private readonly IComplianceRepository _repository;
    private readonly ICompanyRepository _companyRepository;

    public ComplianceService(IComplianceRepository repository, ICompanyRepository companyRepository)
    {
        _repository = repository;
        _companyRepository = companyRepository;
    }

    public async Task<PagedResult<ComplianceResponse>> GetAllAsync(int page, int pageSize)
    {
        var (items, total) = await _repository.GetAllAsync(page, pageSize);
        return new PagedResult<ComplianceResponse>
        {
            Items = items.Select(ToResponse),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<ComplianceResponse>> GetByCompanyAsync(long companyId, int page, int pageSize)
    {
        _ = await _companyRepository.GetByIdAsync(companyId)
            ?? throw new NotFoundException(nameof(Company), companyId);

        var (items, total) = await _repository.GetByCompanyAsync(companyId, page, pageSize);
        return new PagedResult<ComplianceResponse>
        {
            Items = items.Select(ToResponse),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ComplianceResponse> CreateAsync(ComplianceRequest request)
    {
        _ = await _companyRepository.GetByIdAsync(request.CompanyId)
            ?? throw new NotFoundException(nameof(Company), request.CompanyId);

        var record = new ComplianceRecord
        {
            CompanyId = request.CompanyId,
            ComplianceType = request.ComplianceType,
            Status = request.Status,
            ReportDate = request.ReportDate
        };

        var created = await _repository.AddAsync(record);
        return ToResponse(created);
    }

    public async Task<ComplianceResponse> UpdateAsync(long id, ComplianceRequest request)
    {
        var record = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(ComplianceRecord), id);

        _ = await _companyRepository.GetByIdAsync(request.CompanyId)
            ?? throw new NotFoundException(nameof(Company), request.CompanyId);

        record.CompanyId = request.CompanyId;
        record.ComplianceType = request.ComplianceType;
        record.Status = request.Status;
        record.ReportDate = request.ReportDate;

        var updated = await _repository.UpdateAsync(record);
        return ToResponse(updated);
    }

    public async Task DeleteAsync(long id)
    {
        var record = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(ComplianceRecord), id);
        await _repository.DeleteAsync(record);
    }

    private static ComplianceResponse ToResponse(ComplianceRecord r) => new()
    {
        Id = r.Id,
        CompanyId = r.CompanyId,
        CompanyName = r.Company?.Name ?? string.Empty,
        ComplianceType = r.ComplianceType,
        Status = r.Status,
        ReportDate = r.ReportDate
    };
}
