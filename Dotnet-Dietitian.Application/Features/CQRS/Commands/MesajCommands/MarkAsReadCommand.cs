using MediatR;
using System;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.MesajCommands
{
    public class MarkAsReadCommand : IRequest<Unit>
    {
        public Guid MesajId { get; set; }
        public Guid OkuyanId { get; set; }
        public string OkuyanTipi { get; set; }
        
        public MarkAsReadCommand(Guid mesajId, Guid okuyanId, string okuyanTipi)
        {
            MesajId = mesajId;
            OkuyanId = okuyanId;
            OkuyanTipi = okuyanTipi;
        }
    }
}