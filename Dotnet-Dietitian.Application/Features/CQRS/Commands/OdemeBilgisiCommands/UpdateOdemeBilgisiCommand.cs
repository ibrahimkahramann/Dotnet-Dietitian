using MediatR;
using System;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.OdemeBilgisiCommands
{
    public class UpdateOdemeBilgisiCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public Guid HastaId { get; set; }
        public decimal Tutar { get; set; }
        public DateTime Tarih { get; set; }
        public string? OdemeTuru { get; set; }
        public string? Aciklama { get; set; }
        public string? IslemReferansNo { get; set; }
    }
}