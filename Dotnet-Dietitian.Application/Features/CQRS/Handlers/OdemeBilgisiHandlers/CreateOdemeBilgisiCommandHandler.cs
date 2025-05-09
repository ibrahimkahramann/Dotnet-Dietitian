using Dotnet_Dietitian.Application.Features.CQRS.Commands.OdemeBilgisiCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.OdemeBilgisiHandlers
{
    public class CreateOdemeBilgisiCommandHandler : IRequestHandler<CreateOdemeBilgisiCommand, Unit>
    {
        private readonly IRepository<OdemeBilgisi> _repository;
        private readonly IRepository<Hasta> _hastaRepository;

        public CreateOdemeBilgisiCommandHandler(IRepository<OdemeBilgisi> repository, IRepository<Hasta> hastaRepository)
        {
            _repository = repository;
            _hastaRepository = hastaRepository;
        }

        public async Task<Unit> Handle(CreateOdemeBilgisiCommand request, CancellationToken cancellationToken)
        {
            // Hasta mevcut mu kontrolü
            var hasta = await _hastaRepository.GetByIdAsync(request.HastaId);
            if (hasta == null)
                throw new Exception($"ID:{request.HastaId} olan hasta bulunamadı");

            await _repository.AddAsync(new OdemeBilgisi
            {
                HastaId = request.HastaId,
                Tutar = request.Tutar,
                Tarih = request.Tarih,
                OdemeTuru = request.OdemeTuru,
                Aciklama = request.Aciklama,
                IslemReferansNo = request.IslemReferansNo
            });

            return Unit.Value;
        }
    }
}