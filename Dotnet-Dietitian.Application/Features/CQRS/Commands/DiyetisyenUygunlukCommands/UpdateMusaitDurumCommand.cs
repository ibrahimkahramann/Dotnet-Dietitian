using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetisyenUygunlukCommands
{
    public class UpdateMusaitDurumCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public bool Musait { get; set; } // Muayit -> Musait

        public UpdateMusaitDurumCommand(Guid id, bool musait)
        {
            Id = id;
            Musait = musait;
        }
    }
}