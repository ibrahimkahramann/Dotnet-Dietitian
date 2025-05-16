using Dotnet_Dietitian.Application.Features.CQRS.Commands.MesajCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Events;
using MassTransit;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.MesajHandlers
{
    public class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand, Unit>
    {
        private readonly IMesajRepository _mesajRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public MarkAsReadCommandHandler(
            IMesajRepository mesajRepository,
            IPublishEndpoint publishEndpoint)
        {
            _mesajRepository = mesajRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Unit> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
        {
            var result = await _mesajRepository.MarkAsReadAsync(request.MesajId);
            
            if (result > 0)
            {
                // MassTransit ile olayı yayınla
                await _publishEndpoint.Publish<MesajOkunduEvent>(new
                {
                    MesajId = request.MesajId,
                    OkuyanId = request.OkuyanId,
                    OkuyanTipi = request.OkuyanTipi,
                    OkunmaZamani = DateTime.Now
                }, cancellationToken);
            }
            
            return Unit.Value;
        }
    }
}