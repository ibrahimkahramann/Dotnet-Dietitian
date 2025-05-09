using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.RandevuCommands
{
    public class UpdateRandevuCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public Guid HastaId { get; set; }
        public Guid DiyetisyenId { get; set; }
        public DateTime RandevuBaslangicTarihi { get; set; }
        public DateTime RandevuBitisTarihi { get; set; }
        public string? RandevuTuru { get; set; }
        public string Durum { get; set; }
        public bool DiyetisyenOnayi { get; set; }
        public bool HastaOnayi { get; set; }
        public string? Notlar { get; set; }
    }
}