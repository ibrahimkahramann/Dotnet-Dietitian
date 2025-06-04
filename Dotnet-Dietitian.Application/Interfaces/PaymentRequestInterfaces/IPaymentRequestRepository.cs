using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.Interfaces.PaymentRequestInterfaces;

public interface IPaymentRequestRepository
{
    Task<PaymentRequest> GetByIdAsync(Guid id);
    Task<List<PaymentRequest>> GetByHastaIdAsync(Guid hastaId);
    Task<List<PaymentRequest>> GetByDiyetisyenIdAsync(Guid diyetisyenId);
    Task<List<PaymentRequest>> GetAllAsync();
    Task<PaymentRequest> CreateAsync(PaymentRequest paymentRequest);
    Task UpdateAsync(PaymentRequest paymentRequest);
    Task DeleteAsync(Guid id);
}
