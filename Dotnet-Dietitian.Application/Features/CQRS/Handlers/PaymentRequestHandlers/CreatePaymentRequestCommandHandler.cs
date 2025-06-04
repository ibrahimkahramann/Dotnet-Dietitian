using MediatR;
using Dotnet_Dietitian.Application.Features.CQRS.Commands.PaymentRequestCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.PaymentRequestHandlers;

public class CreatePaymentRequestCommandHandler : IRequestHandler<CreatePaymentRequestCommand, Guid>
{
    private readonly IPaymentRequestRepository _repository;

    public CreatePaymentRequestCommandHandler(IPaymentRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreatePaymentRequestCommand request, CancellationToken cancellationToken)
    {
        var paymentRequest = new PaymentRequest
        {
            Id = Guid.NewGuid(),
            HastaId = request.HastaId,
            DiyetisyenId = request.DiyetisyenId,
            DiyetProgramiId = request.DiyetProgramiId,
            Tutar = request.Tutar,
            VadeTarihi = request.VadeTarihi,
            Aciklama = request.Aciklama,
            Durum = PaymentRequestStatus.Bekliyor,
            OlusturulmaTarihi = DateTime.Now
        };        await _repository.AddAsync(paymentRequest);
        return paymentRequest.Id;
    }
}
