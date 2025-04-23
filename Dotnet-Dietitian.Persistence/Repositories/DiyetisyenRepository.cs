using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using Dotnet_Dietitian.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dotnet_Dietitian.Persistence.Repositories;
public class DiyetisyenRepository : BaseRepository<Diyetisyen>, IDiyetisyenRepository
{
    public DiyetisyenRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    public async Task<IReadOnlyList<Diyetisyen>> GetDiyetisyenlerBySehirAsync(string sehir)
    {
        return await _context.Diyetisyenler
            .Where(d => d.Sehir == sehir)
            .ToListAsync();
    }
    
    public async Task<Diyetisyen> GetDiyetisyenWithHastalarAsync(Guid id)
    {
        return await _context.Diyetisyenler
            .Include(d => d.Hastalar)
            .FirstOrDefaultAsync(d => d.Id == id);
    }
}