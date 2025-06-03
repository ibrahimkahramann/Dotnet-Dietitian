using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.Interfaces;

public interface IPaymentRequestRepository : IRepository<PaymentRequest>
{
    Task<List<PaymentRequest>> GetByHastaIdAsync(Guid hastaId);
    Task<List<PaymentRequest>> GetByDiyetisyenIdAsync(Guid diyetisyenId);
}
