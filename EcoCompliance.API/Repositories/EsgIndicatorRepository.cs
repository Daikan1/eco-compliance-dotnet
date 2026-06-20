using EcoCompliance.API.Data;
using EcoCompliance.API.Models;
using EcoCompliance.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcoCompliance.API.Repositories;

public class EsgIndicatorRepository : IEsgIndicatorRepository
{
    private readonly AppDbContext _context;

    public EsgIndicatorRepository(AppDbContext context) => _context = context;

    public async Task<(IEnumerable<EsgIndicator> Items, int TotalCount)> GetAllAsync(int page, int pageSize)
    {
        var query = _context.EsgIndicators.Include(i => i.Company).AsNoTracking();
        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<EsgIndicator> AddAsync(EsgIndicator indicator)
    {
        _context.EsgIndicators.Add(indicator);
        await _context.SaveChangesAsync();
        await _context.Entry(indicator).Reference(i => i.Company).LoadAsync();
        return indicator;
    }
}
