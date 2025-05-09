using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using Dotnet_Dietitian.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dotnet_Dietitian.Persistence.Repositories
{
    public class DiyetisyenUygunlukRepository : BaseRepository<DiyetisyenUygunluk>, IDiyetisyenUygunlukRepository
    {
        public DiyetisyenUygunlukRepository(ApplicationDbContext context) : base(context)
        {
        }
        
        public async Task<IReadOnlyList<DiyetisyenUygunluk>> GetMuayitSlotlarByDiyetisyenIdAsync(Guid diyetisyenId)
        {
            return await _context.DiyetisyenUygunluklar
                .Where(du => du.DiyetisyenId == diyetisyenId && du.Muayit)
                .Include(du => du.Diyetisyen)
                .ToListAsync();
        }
        
        public async Task<IReadOnlyList<DiyetisyenUygunluk>> GetUygunlukByTarihAraligindaAsync(DateTime baslangic, DateTime bitis)
        {
            return await _context.DiyetisyenUygunluklar
                .Where(du => 
                    (du.BaslangicZamani >= baslangic && du.BaslangicZamani <= bitis) || 
                    (du.BitisZamani >= baslangic && du.BitisZamani <= bitis) ||
                    (du.BaslangicZamani <= baslangic && du.BitisZamani >= bitis))
                .Include(du => du.Diyetisyen)
                .ToListAsync();
        }
    }
}