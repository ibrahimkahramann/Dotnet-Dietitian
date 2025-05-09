using Dotnet_Dietitian.Application.Features.CQRS.Commands.OdemeBilgisiCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.OdemeBilgisiHandlers
{
    public class UpdateOdemeBilgisiCommandHandler : IRequestHandler<UpdateOdemeBilgisiCommand, Unit>
    {
        private readonly IRepository<OdemeBilgisi> _repository;
        private readonly IRepository<Hasta> _hastaRepository;

        public UpdateOdemeBilgisiCommandHandler(IRepository<OdemeBilgisi> repository, IRepository<Hasta> hastaRepository)
        {
            _repository = repository;
            _hastaRepository = hastaRepository;
        }

        public async Task<Unit> Handle(UpdateOdemeBilgisiCommand request, CancellationToken cancellationToken)
        {
            var odeme = await _repository.GetByIdAsync(request.Id);
            if (odeme == null)
                throw new Exception($"ID:{request.Id} olan ödeme bilgisi bulunamadı");

            // Hasta mevcut mu kontrolü
            var hasta = await _hastaRepository.GetByIdAsync(request.HastaId);
            if (hasta == null)
                throw new Exception($"ID:{request.HastaId} olan hasta bulunamadı");

            odeme.HastaId = request.HastaId;
            odeme.Tutar = request.Tutar;
            odeme.Tarih = request.Tarih;
            odeme.OdemeTuru = request.OdemeTuru;
            odeme.Aciklama = request.Aciklama;
            odeme.IslemReferansNo = request.IslemReferansNo;

            await _repository.UpdateAsync(odeme);
            return Unit.Value;
        }
    }
}