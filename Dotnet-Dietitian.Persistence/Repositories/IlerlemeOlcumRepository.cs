using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using Dotnet_Dietitian.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Persistence.Repositories
{
    public class IlerlemeOlcumRepository : BaseRepository<IlerlemeOlcum>, IIlerlemeOlcumRepository
    {
        public IlerlemeOlcumRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<IlerlemeOlcum>> GetIlerlemeOlcumlerByHastaIdAsync(Guid hastaId)
        {
            return await _context.IlerlemeOlcumleri
                .Where(i => i.HastaId == hastaId)
                .OrderByDescending(i => i.OlcumTarihi)
                .ToListAsync();
        }

        public async Task<IlerlemeOlcum> GetSonIlerlemeOlcumByHastaIdAsync(Guid hastaId)
        {
            return await _context.IlerlemeOlcumleri
                .Where(i => i.HastaId == hastaId)
                .OrderByDescending(i => i.OlcumTarihi)
                .FirstOrDefaultAsync();
        }
    }
} 