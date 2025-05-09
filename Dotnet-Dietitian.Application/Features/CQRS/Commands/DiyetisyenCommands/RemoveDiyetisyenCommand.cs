using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetisyenCommands
{
    public class RemoveDiyetisyenCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }

        public RemoveDiyetisyenCommand(Guid id)
        {
            Id = id;
        }
    }
}