using MediatR;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.PaymentRequestQueries;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.PaymentRequestHandlers;

public class GetPaymentRequestByIdQueryHandler : IRequestHandler<GetPaymentRequestByIdQuery, PaymentRequest>
{
    private readonly IPaymentRequestRepository _repository;

    public GetPaymentRequestByIdQueryHandler(IPaymentRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaymentRequest> Handle(GetPaymentRequestByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id);
    }
}
