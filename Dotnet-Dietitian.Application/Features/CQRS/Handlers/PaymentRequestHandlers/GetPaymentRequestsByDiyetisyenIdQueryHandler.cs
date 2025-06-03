using MediatR;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.PaymentRequestQueries;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.PaymentRequestHandlers;

public class GetPaymentRequestsByDiyetisyenIdQueryHandler : IRequestHandler<GetPaymentRequestsByDiyetisyenIdQuery, List<PaymentRequest>>
{
    private readonly IPaymentRequestRepository _repository;

    public GetPaymentRequestsByDiyetisyenIdQueryHandler(IPaymentRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<PaymentRequest>> Handle(GetPaymentRequestsByDiyetisyenIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByDiyetisyenIdAsync(request.DiyetisyenId);
    }
}
