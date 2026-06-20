using EcoCompliance.API.Data;
using EcoCompliance.API.Models;
using EcoCompliance.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcoCompliance.API.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly AppDbContext _context;

    public CompanyRepository(AppDbContext context) => _context = context;

    public async Task<(IEnumerable<Company> Items, int TotalCount)> GetAllAsync(int page, int pageSize)
    {
        var query = _context.Companies.AsNoTracking();
        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Company?> GetByIdAsync(long id) =>
        await _context.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Company> AddAsync(Company company)
    {
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();
        return company;
    }
}
