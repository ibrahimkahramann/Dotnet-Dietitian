using MediatR;
using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.PaymentRequestCommands;

public class UpdatePaymentRequestStatusCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public PaymentRequestStatus Durum { get; set; }
    public string? RedNotu { get; set; }
    public Guid? OdemeBilgisiId { get; set; }
}
