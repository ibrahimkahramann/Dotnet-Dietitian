using Dotnet_Dietitian.Application.Features.CQRS.Queries.RandevuQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Results.RandevuResults;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.RandevuHandlers
{
    public class GetRandevuQueryHandler : IRequestHandler<GetRandevuQuery, List<GetRandevuQueryResult>>
    {
        private readonly IRepository<Randevu> _repository;
        private readonly IRepository<Hasta> _hastaRepository;
        private readonly IRepository<Diyetisyen> _diyetisyenRepository;

        public GetRandevuQueryHandler(
            IRepository<Randevu> repository, 
            IRepository<Hasta> hastaRepository, 
            IRepository<Diyetisyen> diyetisyenRepository)
        {
            _repository = repository;
            _hastaRepository = hastaRepository;
            _diyetisyenRepository = diyetisyenRepository;
        }

        public async Task<List<GetRandevuQueryResult>> Handle(GetRandevuQuery request, CancellationToken cancellationToken)
        {
            var randevular = await _repository.GetAllAsync();
            var results = new List<GetRandevuQueryResult>();

            foreach (var randevu in randevular)
            {
                var hasta = await _hastaRepository.GetByIdAsync(randevu.HastaId);
                var diyetisyen = await _diyetisyenRepository.GetByIdAsync(randevu.DiyetisyenId);
                
                var result = new GetRandevuQueryResult
                {
                    Id = randevu.Id,
                    HastaId = randevu.HastaId,
                    HastaAdSoyad = hasta != null ? $"{hasta.Ad} {hasta.Soyad}" : "Bilinmiyor",
                    DiyetisyenId = randevu.DiyetisyenId,
                    DiyetisyenAdSoyad = diyetisyen != null ? $"{diyetisyen.Ad} {diyetisyen.Soyad}" : "Bilinmiyor",
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