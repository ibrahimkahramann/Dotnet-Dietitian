using Dotnet_Dietitian.Application.Features.CQRS.Commands.RandevuCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.RandevuHandlers
{
    public class CreateRandevuCommandHandler : IRequestHandler<CreateRandevuCommand, Unit>
    {
        private readonly IRepository<Randevu> _repository;
        private readonly IRepository<Diyetisyen> _diyetisyenRepository;
        private readonly IRepository<Hasta> _hastaRepository;
        private readonly IAppConfigService _appConfigService;

        public CreateRandevuCommandHandler(
            IRepository<Randevu> repository,
            IRepository<Diyetisyen> diyetisyenRepository,
            IRepository<Hasta> hastaRepository,
            IAppConfigService appConfigService)
        {
            _repository = repository;
            _diyetisyenRepository = diyetisyenRepository;
            _hastaRepository = hastaRepository;
            _appConfigService = appConfigService;
        }

        public async Task<Unit> Handle(CreateRandevuCommand request, CancellationToken cancellationToken)
        {
            // Önce hasta ve diyetisyen varlığını kontrol et
            var diyetisyen = await _diyetisyenRepository.GetByIdAsync(request.DiyetisyenId);
            if (diyetisyen == null)
                throw new Exception($"ID:{request.DiyetisyenId} olan diyetisyen bulunamadı");

            var hasta = await _hastaRepository.GetByIdAsync(request.HastaId);
            if (hasta == null)
                throw new Exception($"ID:{request.HastaId} olan hasta bulunamadı");

            // Günlük maksimum randevu kontrolü - Singleton servis kullanımı
            var today = DateTime.Today;
            var dailyAppointments = (await _repository.GetAsync(r => 
                r.DiyetisyenId == request.DiyetisyenId && 
                r.RandevuBaslangicTarihi.Date == request.RandevuBaslangicTarihi.Date)).Count();

            if (dailyAppointments >= _appConfigService.MaxDailyAppointments)
            {
                throw new Exception($"Bu diyetisyen için günlük maksimum randevu sayısına ({_appConfigService.MaxDailyAppointments}) ulaşıldı.");
            }

            // Randevu süresinin kontrolü
            var duration = request.RandevuBitisTarihi - request.RandevuBaslangicTarihi;
            if (duration.TotalMinutes > _appConfigService.DefaultSessionDurationMinutes)
            {
                throw new Exception($"Randevu süresi maksimum {_appConfigService.DefaultSessionDurationMinutes} dakika olabilir.");
            }

            await _repository.AddAsync(new Randevu
            {
                HastaId = request.HastaId,
                DiyetisyenId = request.DiyetisyenId,
                RandevuBaslangicTarihi = request.RandevuBaslangicTarihi,
                RandevuBitisTarihi = request.RandevuBitisTarihi,
                RandevuTuru = request.RandevuTuru,
                Durum = "Bekliyor",
                DiyetisyenOnayi = false,
                HastaOnayi = true,  // Hasta randevuyu kendisi oluşturduğu için otomatik onaylı
                Notlar = request.Notlar,
                YaratilmaTarihi = DateTime.Now
            });

            return Unit.Value;
        }
    }
}