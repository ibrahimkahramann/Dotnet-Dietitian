using Dotnet_Dietitian.Application.Features.CQRS.Commands.RandevuCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.RandevuHandlers
{
    public class UpdateRandevuCommandHandler : IRequestHandler<UpdateRandevuCommand, Unit>
    {
        private readonly IRepository<Randevu> _repository;

        public UpdateRandevuCommandHandler(IRepository<Randevu> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateRandevuCommand request, CancellationToken cancellationToken)
        {
            var randevu = await _repository.GetByIdAsync(request.Id);
            if (randevu == null)
                throw new Exception($"ID:{request.Id} olan randevu bulunamadı");

            randevu.HastaId = request.HastaId;
            randevu.DiyetisyenId = request.DiyetisyenId;
            randevu.RandevuBaslangicTarihi = request.RandevuBaslangicTarihi;
            randevu.RandevuBitisTarihi = request.RandevuBitisTarihi;
            randevu.RandevuTuru = request.RandevuTuru;
            randevu.Durum = request.Durum;
            randevu.DiyetisyenOnayi = request.DiyetisyenOnayi;
            randevu.HastaOnayi = request.HastaOnayi;
            randevu.Notlar = request.Notlar;

            await _repository.UpdateAsync(randevu);
            return Unit.Value;
        }
    }
}