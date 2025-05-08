using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using Dotnet_Dietitian.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dotnet_Dietitian.Persistence.Repositories
{
    public class RandevuRepository : BaseRepository<Randevu>, IRandevuRepository
    {
        public RandevuRepository(ApplicationDbContext context) : base(context)
        {
        }
        
        public async Task<IReadOnlyList<Randevu>> GetRandevularByTarihAraligindaAsync(DateTime baslangic, DateTime bitis)
        {
            return await _context.Randevular
                .Where(r => r.RandevuBaslangicTarihi >= baslangic && r.RandevuBitisTarihi <= bitis)
                .Include(r => r.Hasta)
                .Include(r => r.Diyetisyen)
                .ToListAsync();
        }
        
        public async Task<IReadOnlyList<Randevu>> GetOnayBekleyenRandevularAsync()
        {
            return await _context.Randevular
                .Where(r => r.Durum == "Bekliyor" && (!r.DiyetisyenOnayi || !r.HastaOnayi))
                .Include(r => r.Hasta)
                .Include(r => r.Diyetisyen)
                .ToListAsync();
        }
    }
}