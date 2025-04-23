using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using Dotnet_Dietitian.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dotnet_Dietitian.Persistence.Repositories;
    public class HastaRepository : BaseRepository<Hasta>, IHastaRepository
    {
        public HastaRepository(ApplicationDbContext context) : base(context)
        {
        }
        
        public async Task<IReadOnlyList<Hasta>> GetHastasByDiyetisyenIdAsync(Guid diyetisyenId)
        {
            return await _context.Hastalar
                .Where(h => h.DiyetisyenId == diyetisyenId)
                .ToListAsync();
        }
        
        public async Task<Hasta> GetHastaWithDiyetProgramiAsync(Guid id)
        {
            return await _context.Hastalar
                .Include(h => h.DiyetProgrami)
                .FirstOrDefaultAsync(h => h.Id == id);
        }
    }