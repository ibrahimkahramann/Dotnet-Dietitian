using Dotnet_Dietitian.Application.Features.CQRS.Queries.RandevuQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Results.RandevuResults;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.RandevuHandlers
{
    public class GetRandevuByDiyetisyenIdQueryHandler : IRequestHandler<GetRandevuByDiyetisyenIdQuery, List<GetRandevuQueryResult>>
    {
        private readonly IRepository<Randevu> _repository;
        private readonly IRepository<Hasta> _hastaRepository;
        private readonly IRepository<Diyetisyen> _diyetisyenRepository;

        public GetRandevuByDiyetisyenIdQueryHandler(
            IRepository<Randevu> repository,
            IRepository<Hasta> hastaRepository,
            IRepository<Diyetisyen> diyetisyenRepository)
        {
            _repository = repository;
            _hastaRepository = hastaRepository;
            _diyetisyenRepository = diyetisyenRepository;
        }

        public async Task<List<GetRandevuQueryResult>> Handle(GetRandevuByDiyetisyenIdQuery request, CancellationToken cancellationToken)
        {
            var diyetisyen = await _diyetisyenRepository.GetByIdAsync(request.DiyetisyenId);
            if (diyetisyen == null)
                throw new Exception($"ID:{request.DiyetisyenId} olan diyetisyen bulunamadı");

            var randevular = await _repository.GetAsync(r => r.DiyetisyenId == request.DiyetisyenId);
            var results = new List<GetRandevuQueryResult>();

            foreach (var randevu in randevular)
            {
                var hasta = await _hastaRepository.GetByIdAsync(randevu.HastaId);

                var result = new GetRandevuQueryResult
                {
                    Id = randevu.Id,
                    HastaId = randevu.HastaId,
                    HastaAdSoyad = hasta != null ? $"{hasta.Ad} {hasta.Soyad}" : "Bilinmiyor",
                    DiyetisyenId = randevu.DiyetisyenId,
                    DiyetisyenAdSoyad = $"{diyetisyen.Ad} {diyetisyen.Soyad}",
                    RandevuBaslangicTarihi = randevu.RandevuBaslangicTarihi,
                    RandevuBitisTarihi = randevu.RandevuBitisTarihi,
                    RandevuTuru = randevu.RandevuTuru,
                    Durum = randevu.Durum,
                    DiyetisyenOnayi = randevu.DiyetisyenOnayi,
                    HastaOnayi = randevu.HastaOnayi,
                    Notlar = randevu.Notlar,
                    YaratilmaTarihi = randevu.YaratilmaTarihi
                };

                results.Add(result);
            }

            return results;
        }
    }
}