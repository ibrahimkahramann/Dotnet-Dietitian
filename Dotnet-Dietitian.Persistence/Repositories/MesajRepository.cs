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
    public class MesajRepository : BaseRepository<Mesaj>, IMesajRepository
    {
        public MesajRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Mesaj>> GetConversationAsync(Guid user1Id, string user1Type, Guid user2Id, string user2Type, int count = 50)
        {
            return await _context.Mesajlar
                .Where(m => 
                    (m.GonderenId == user1Id && m.GonderenTipi == user1Type && m.AliciId == user2Id && m.AliciTipi == user2Type) || 
                    (m.GonderenId == user2Id && m.GonderenTipi == user2Type && m.AliciId == user1Id && m.AliciTipi == user1Type))
                .OrderByDescending(m => m.GonderimZamani)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Mesaj>> GetUnreadMessagesAsync(Guid userId, string userType)
        {
            return await _context.Mesajlar
                .Where(m => m.AliciId == userId && m.AliciTipi == userType && !m.Okundu)
                .OrderByDescending(m => m.GonderimZamani)
                .ToListAsync();
        }

        public async Task<int> MarkAsReadAsync(Guid mesajId)
        {
            var mesaj = await _context.Mesajlar.FindAsync(mesajId);
            if (mesaj != null && !mesaj.Okundu)
            {
                mesaj.Okundu = true;
                mesaj.OkunmaZamani = DateTime.Now;
                return await _context.SaveChangesAsync();
            }
            return 0;
        }
    }
}