using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using Dotnet_Dietitian.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dotnet_Dietitian.Persistence.Repositories;

public class PaymentRequestRepository : BaseRepository<PaymentRequest>, IPaymentRequestRepository
{
    public PaymentRequestRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<PaymentRequest>> GetByHastaIdAsync(Guid hastaId)
    {
        return await _context.PaymentRequests
            .Include(pr => pr.Hasta)
            .Include(pr => pr.Diyetisyen)
            .Include(pr => pr.DiyetProgrami)
            .Include(pr => pr.OdemeBilgisi)
            .Where(pr => pr.HastaId == hastaId)
            .OrderByDescending(pr => pr.OlusturulmaTarihi)
            .ToListAsync();
    }

    public async Task<List<PaymentRequest>> GetByDiyetisyenIdAsync(Guid diyetisyenId)
    {
        return await _context.PaymentRequests
            .Include(pr => pr.Hasta)
            .Include(pr => pr.Diyetisyen)
            .Include(pr => pr.DiyetProgrami)
            .Include(pr => pr.OdemeBilgisi)
            .Where(pr => pr.DiyetisyenId == diyetisyenId)
            .OrderByDescending(pr => pr.OlusturulmaTarihi)
            .ToListAsync();
    }
}
