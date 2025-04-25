using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using Dotnet_Dietitian.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dotnet_Dietitian.Persistence.Repositories;

public class DiyetProgramiRepository : BaseRepository<DiyetProgrami>, IDiyetProgramiRepository
{
    public DiyetProgramiRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    public async Task<IReadOnlyList<DiyetProgrami>> GetDiyetProgramiByDiyetisyenIdAsync(Guid diyetisyenId)
    {
        return await _context.DiyetProgramlari
            .Where(dp => dp.OlusturanDiyetisyenId == diyetisyenId)
            .ToListAsync();
    }
    
    public async Task<DiyetProgrami> GetDiyetProgramiWithHastalarAsync(Guid id)
    {
        return await _context.DiyetProgramlari
            .Include(dp => dp.Hastalar)
            .FirstOrDefaultAsync(dp => dp.Id == id);
    }
}
