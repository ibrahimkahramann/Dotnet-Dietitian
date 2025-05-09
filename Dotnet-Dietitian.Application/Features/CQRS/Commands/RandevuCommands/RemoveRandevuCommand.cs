using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.RandevuCommands
{
    public class RemoveRandevuCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }

        public RemoveRandevuCommand(Guid id)
        {
            Id = id;
        }
    }
}