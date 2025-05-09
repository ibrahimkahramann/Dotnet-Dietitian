using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetProgramiCommands
{
    public class CreateDiyetProgramiCommand : IRequest<Unit>
    {
        public string Ad { get; set; }
        public string? Aciklama { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public decimal? YagGram { get; set; }
        public decimal? ProteinGram { get; set; }
        public decimal? KarbonhidratGram { get; set; }
        public int? GunlukAdimHedefi { get; set; }
        public Guid? OlusturanDiyetisyenId { get; set; }
    }
}