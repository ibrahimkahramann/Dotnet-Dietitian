using MediatR;
using System;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.OdemeBilgisiCommands
{
    public class CreateOdemeBilgisiCommand : IRequest<Unit>
    {
        public Guid HastaId { get; set; }
        public decimal Tutar { get; set; }
        public DateTime Tarih { get; set; } = DateTime.Now;
        public string? OdemeTuru { get; set; }
        public string? Aciklama { get; set; }
        public string? IslemReferansNo { get; set; }
    }
}