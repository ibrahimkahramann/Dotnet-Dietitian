using Dotnet_Dietitian.Application.Features.CQRS.Commands.RandevuCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.RandevuHandlers
{
    public class CreateRandevuCommandHandler : IRequestHandler<CreateRandevuCommand, Unit>
    {
        private readonly IRepository<Randevu> _repository;
        private readonly IRepository<Hasta> _hastaRepository;
        private readonly IRepository<Diyetisyen> _diyetisyenRepository;

        public CreateRandevuCommandHandler(
            IRepository<Randevu> repository,
            IRepository<Hasta> hastaRepository,
            IRepository<Diyetisyen> diyetisyenRepository)
        {
            _repository = repository;
            _hastaRepository = hastaRepository;
            _diyetisyenRepository = diyetisyenRepository;
        }

        public async Task<Unit> Handle(CreateRandevuCommand request, CancellationToken cancellationToken)
        {
            // Hasta ve Diyetisyen mevcut mu kontrolü
            var hasta = await _hastaRepository.GetByIdAsync(request.HastaId);
            if (hasta == null)
                throw new Exception($"ID:{request.HastaId} olan hasta bulunamadı");

            var diyetisyen = await _diyetisyenRepository.GetByIdAsync(request.DiyetisyenId);
            if (diyetisyen == null)
                throw new Exception($"ID:{request.DiyetisyenId} olan diyetisyen bulunamadı");

            // Yeni randevu oluştur
            var randevu = new Randevu
            {
                HastaId = request.HastaId,
                DiyetisyenId = request.DiyetisyenId,
                RandevuBaslangicTarihi = request.RandevuBaslangicTarihi,
                RandevuBitisTarihi = request.RandevuBitisTarihi,
                RandevuTuru = request.RandevuTuru,
                Durum = request.Durum,
                DiyetisyenOnayi = request.DiyetisyenOnayi,
                HastaOnayi = request.HastaOnayi,
                Notlar = request.Notlar,
                YaratilmaTarihi = request.YaratilmaTarihi
            };

            await _repository.AddAsync(randevu);
            return Unit.Value;
        }
    }
}