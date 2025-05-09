using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetisyenUygunlukCommands
{
    public class RemoveDiyetisyenUygunlukCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }

        public RemoveDiyetisyenUygunlukCommand(Guid id)
        {
            Id = id;
        }
    }
}