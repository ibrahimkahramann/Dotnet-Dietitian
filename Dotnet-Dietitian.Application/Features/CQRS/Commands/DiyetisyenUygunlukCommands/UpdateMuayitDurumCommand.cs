using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetisyenUygunlukCommands
{
    public class UpdateMuayitDurumCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public bool Muayit { get; set; }

        public UpdateMuayitDurumCommand(Guid id, bool muayit)
        {
            Id = id;
            Muayit = muayit;
        }
    }
}