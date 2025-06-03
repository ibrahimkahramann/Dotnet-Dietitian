using MediatR;
using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.PaymentRequestQueries;

public class GetPaymentRequestsByHastaIdQuery : IRequest<List<PaymentRequest>>
{
    public Guid HastaId { get; set; }
}
