using MediatR;
using System;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.OdemeBilgisiCommands
{
    public class RemoveOdemeBilgisiCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }

        public RemoveOdemeBilgisiCommand(Guid id)
        {
            Id = id;
        }
    }
}