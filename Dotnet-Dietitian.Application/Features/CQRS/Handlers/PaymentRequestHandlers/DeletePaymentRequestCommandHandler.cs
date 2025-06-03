using MediatR;
using Dotnet_Dietitian.Application.Features.CQRS.Commands.PaymentRequestCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.PaymentRequestHandlers;

public class DeletePaymentRequestCommandHandler : IRequestHandler<DeletePaymentRequestCommand>
{
    private readonly IPaymentRequestRepository _repository;

    public DeletePaymentRequestCommandHandler(IPaymentRequestRepository repository)
    {
        _repository = repository;
    }    public async Task<Unit> Handle(DeletePaymentRequestCommand request, CancellationToken cancellationToken)
    {
        var paymentRequest = await _repository.GetByIdAsync(request.Id);
        if (paymentRequest != null)
        {
            await _repository.DeleteAsync(paymentRequest);
        }
        return Unit.Value;
    }
}
