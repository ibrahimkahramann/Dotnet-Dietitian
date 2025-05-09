using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.HastaCommands
{
    public class RemoveHastaCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }

        public RemoveHastaCommand(Guid id)
        {
            Id = id;
        }
    }
}