using Dotnet_Dietitian.Application.Features.CQRS.Queries.RandevuQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Results.RandevuResults;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.RandevuHandlers
{
    public class GetRandevuByIdQueryHandler : IRequestHandler<GetRandevuByIdQuery, GetRandevuByIdQueryResult>
    {
        private readonly IRepository<Randevu> _repository;
        private readonly IRepository<Hasta> _hastaRepository;
        private readonly IRepository<Diyetisyen> _diyetisyenRepository;

        public GetRandevuByIdQueryHandler(
            IRepository<Randevu> repository,
            IRepository<Hasta> hastaRepository,
            IRepository<Diyetisyen> diyetisyenRepository)
        {
            _repository = repository;
            _hastaRepository = hastaRepository;
            _diyetisyenRepository = diyetisyenRepository;
        }

        public async Task<GetRandevuByIdQueryResult> Handle(GetRandevuByIdQuery request, CancellationToken cancellationToken)
        {
            var randevu = await _repository.GetByIdAsync(request.Id);
            if (randevu == null)
                throw new Exception($"ID:{request.Id} olan randevu bulunamadı");

            var hasta = await _hastaRepository.GetByIdAsync(randevu.HastaId);
            var diyetisyen = await _diyetisyenRepository.GetByIdAsync(randevu.DiyetisyenId);

            var result = new GetRandevuByIdQueryResult
            {
                Id = randevu.Id,
                HastaId = randevu.HastaId,
                HastaAdSoyad = hasta != null ? $"{hasta.Ad} {hasta.Soyad}" : "Bilinmiyor",
                HastaEmail = hasta?.Email,
                HastaTelefon = hasta?.Telefon,
                DiyetisyenId = randevu.DiyetisyenId,
                DiyetisyenAdSoyad = diyetisyen != null ? $"{diyetisyen.Ad} {diyetisyen.Soyad}" : "Bilinmiyor",
                DiyetisyenUzmanlik = diyetisyen?.Uzmanlik,
                DiyetisyenEmail = diyetisyen?.Email,
                DiyetisyenTelefon = diyetisyen?.Telefon,
                RandevuBaslangicTarihi = randevu.RandevuBaslangicTarihi,
                RandevuBitisTarihi = randevu.RandevuBitisTarihi,
                RandevuTuru = randevu.RandevuTuru,
                Durum = randevu.Durum,
                DiyetisyenOnayi = randevu.DiyetisyenOnayi,
                HastaOnayi = randevu.HastaOnayi,
                Notlar = randevu.Notlar,
                YaratilmaTarihi = randevu.YaratilmaTarihi
            };

            return result;
        }
    }
}