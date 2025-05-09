using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.RandevuCommands
{
    public class CreateRandevuCommand : IRequest<Unit>
    {
        public Guid HastaId { get; set; }
        public Guid DiyetisyenId { get; set; }
        public DateTime RandevuBaslangicTarihi { get; set; }
        public DateTime RandevuBitisTarihi { get; set; }
        public string? RandevuTuru { get; set; }
        public string Durum { get; set; } = "Bekliyor";
        public bool DiyetisyenOnayi { get; set; } = false;
        public bool HastaOnayi { get; set; } = false;
        public string? Notlar { get; set; }
        public DateTime YaratilmaTarihi { get; set; } = DateTime.Now;
    }
}