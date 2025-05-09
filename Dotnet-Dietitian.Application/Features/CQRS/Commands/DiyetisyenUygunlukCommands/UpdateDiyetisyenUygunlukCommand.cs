using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetisyenUygunlukCommands
{
    public class UpdateDiyetisyenUygunlukCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public Guid DiyetisyenId { get; set; }
        public DateTime BaslangicZamani { get; set; }
        public DateTime BitisZamani { get; set; }
        public bool Muayit { get; set; }
        public bool TekrarlanirMi { get; set; }
        public string? TekrarlanmaSikligi { get; set; }
        public string? Notlar { get; set; }
    }
}