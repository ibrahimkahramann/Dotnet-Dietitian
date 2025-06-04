using MediatR;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.PaymentRequestQueries;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.PaymentRequestHandlers;

public class GetPaymentRequestsByHastaIdQueryHandler : IRequestHandler<GetPaymentRequestsByHastaIdQuery, List<PaymentRequest>>
{
    private readonly IPaymentRequestRepository _repository;

    public GetPaymentRequestsByHastaIdQueryHandler(IPaymentRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<PaymentRequest>> Handle(GetPaymentRequestsByHastaIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByHastaIdAsync(request.HastaId);
    }
}
