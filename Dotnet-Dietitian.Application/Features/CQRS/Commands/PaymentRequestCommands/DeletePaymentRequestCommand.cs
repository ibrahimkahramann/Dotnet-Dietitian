using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.PaymentRequestCommands;

public class DeletePaymentRequestCommand : IRequest
{
    public Guid Id { get; set; }
}
