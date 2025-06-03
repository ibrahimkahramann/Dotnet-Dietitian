using MediatR;
using Dotnet_Dietitian.Application.Features.CQRS.Commands.PaymentRequestCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.PaymentRequestHandlers;

public class UpdatePaymentRequestStatusCommandHandler : IRequestHandler<UpdatePaymentRequestStatusCommand, bool>
{
    private readonly IPaymentRequestRepository _repository;

    public UpdatePaymentRequestStatusCommandHandler(IPaymentRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(UpdatePaymentRequestStatusCommand request, CancellationToken cancellationToken)
    {
        var paymentRequest = await _repository.GetByIdAsync(request.Id);
        if (paymentRequest == null)
            return false;

        paymentRequest.Durum = request.Durum;
        paymentRequest.RedNotu = request.RedNotu;
        paymentRequest.OdemeBilgisiId = request.OdemeBilgisiId;
        
        if (request.Durum == PaymentRequestStatus.Odendi)
        {
            paymentRequest.OdemeTarihi = DateTime.Now;
        }

        await _repository.UpdateAsync(paymentRequest);
        return true;
    }
}
