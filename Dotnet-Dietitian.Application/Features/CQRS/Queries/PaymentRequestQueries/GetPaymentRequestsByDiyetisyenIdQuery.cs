using MediatR;
using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.PaymentRequestQueries;

public class GetPaymentRequestsByDiyetisyenIdQuery : IRequest<List<PaymentRequest>>
{
    public Guid DiyetisyenId { get; set; }
}
