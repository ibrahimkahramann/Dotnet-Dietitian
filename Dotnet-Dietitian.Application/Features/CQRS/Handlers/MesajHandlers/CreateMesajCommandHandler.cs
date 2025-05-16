using Dotnet_Dietitian.Application.Features.CQRS.Commands.MesajCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using Dotnet_Dietitian.Domain.Events;
using MassTransit;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.MesajHandlers
{
    public class CreateMesajCommandHandler : IRequestHandler<CreateMesajCommand, Guid>
    {
        private readonly IRepository<Mesaj> _repository;
        private readonly IPublishEndpoint _publishEndpoint;

        public CreateMesajCommandHandler(
            IRepository<Mesaj> repository,
            IPublishEndpoint publishEndpoint)
        {
            _repository = repository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Guid> Handle(CreateMesajCommand request, CancellationToken cancellationToken)
        {
            var mesaj = new Mesaj
            {
                Id = Guid.NewGuid(),
                GonderenId = request.GonderenId,
                GonderenTipi = request.GonderenTipi,
                AliciId = request.AliciId,
                AliciTipi = request.AliciTipi,
                Icerik = request.Icerik,
                GonderimZamani = DateTime.Now
            };
            
            await _repository.AddAsync(mesaj);
            
            // MassTransit ile olayı yayınla
            await _publishEndpoint.Publish<MesajGonderildiEvent>(new
            {
                MesajId = mesaj.Id,
                GonderenId = mesaj.GonderenId,
                GonderenTipi = mesaj.GonderenTipi,
                AliciId = mesaj.AliciId,
                AliciTipi = mesaj.AliciTipi,
                GonderimZamani = mesaj.GonderimZamani
            }, cancellationToken);
            
            return mesaj.Id;
        }
    }
}