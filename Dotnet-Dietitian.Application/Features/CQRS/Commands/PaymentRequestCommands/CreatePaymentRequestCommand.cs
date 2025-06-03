using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.PaymentRequestCommands;

public class CreatePaymentRequestCommand : IRequest<Guid>
{
    public Guid HastaId { get; set; }
    public Guid DiyetisyenId { get; set; }
    public Guid DiyetProgramiId { get; set; }
    public decimal Tutar { get; set; }
    public DateTime? VadeTarihi { get; set; }
    public string? Aciklama { get; set; }
}
