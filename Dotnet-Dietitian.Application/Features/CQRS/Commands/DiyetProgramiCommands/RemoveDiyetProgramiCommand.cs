using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetProgramiCommands
{
    public class RemoveDiyetProgramiCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }

        public RemoveDiyetProgramiCommand(Guid id)
        {
            Id = id;
        }
    }
}