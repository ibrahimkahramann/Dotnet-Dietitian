using MediatR;
using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.PaymentRequestQueries;

public class GetPaymentRequestByIdQuery : IRequest<PaymentRequest>
{
    public Guid Id { get; set; }
}
