using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetisyenUygunlukCommands
{
    public class CreateDiyetisyenUygunlukCommand : IRequest<Unit>
    {
        public Guid DiyetisyenId { get; set; }
        public DateTime BaslangicZamani { get; set; }
        public DateTime BitisZamani { get; set; }
        public bool Musait { get; set; } = true;
        public bool TekrarlanirMi { get; set; } = false;
        public string? TekrarlanmaSikligi { get; set; }
        public string? Notlar { get; set; }
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;
    }
}