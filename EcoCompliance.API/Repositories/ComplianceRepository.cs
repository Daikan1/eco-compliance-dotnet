using EcoCompliance.API.Data;
using EcoCompliance.API.Models;
using EcoCompliance.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcoCompliance.API.Repositories;

public class ComplianceRepository : IComplianceRepository
{
    private readonly AppDbContext _context;

    public ComplianceRepository(AppDbContext context) => _context = context;

    public async Task<(IEnumerable<ComplianceRecord> Items, int TotalCount)> GetAllAsync(int page, int pageSize)
    {
        var query = _context.ComplianceRecords.Include(r => r.Company).AsNoTracking();
        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<(IEnumerable<ComplianceRecord> Items, int TotalCount)> GetByCompanyAsync(long companyId, int page, int pageSize)
    {
        var query = _context.ComplianceRecords
            .Include(r => r.Company)
            .Where(r => r.CompanyId == companyId)
            .AsNoTracking();

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<ComplianceRecord?> GetByIdAsync(long id) =>
        await _context.ComplianceRecords.Include(r => r.Company).FirstOrDefaultAsync(r => r.Id == id);

    public async Task<ComplianceRecord> AddAsync(ComplianceRecord record)
    {
        _context.ComplianceRecords.Add(record);
        await _context.SaveChangesAsync();
        await _context.Entry(record).Reference(r => r.Company).LoadAsync();
        return record;
    }

    public async Task<ComplianceRecord> UpdateAsync(ComplianceRecord record)
    {
        _context.ComplianceRecords.Update(record);
        await _context.SaveChangesAsync();
        await _context.Entry(record).Reference(r => r.Company).LoadAsync();
        return record;
    }

    public async Task DeleteAsync(ComplianceRecord record)
    {
        _context.ComplianceRecords.Remove(record);
        await _context.SaveChangesAsync();
    }
}
