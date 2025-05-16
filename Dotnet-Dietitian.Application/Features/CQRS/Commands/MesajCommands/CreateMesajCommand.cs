using MediatR;
using System;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.MesajCommands
{
    public class CreateMesajCommand : IRequest<Guid>
    {
        public Guid GonderenId { get; set; }
        public string GonderenTipi { get; set; }
        public Guid AliciId { get; set; }
        public string AliciTipi { get; set; }
        public string Icerik { get; set; }
    }
}