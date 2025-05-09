using Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetisyenUygunlukQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Results.DiyetisyenUygunlukResults;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.DiyetisyenUygunlukHandlers
{
    public class GetDiyetisyenUygunlukByTarihQueryHandler : IRequestHandler<GetDiyetisyenUygunlukByTarihQuery, List<GetDiyetisyenUygunlukQueryResult>>
    {
        private readonly IRepository<DiyetisyenUygunluk> _repository;
        private readonly IRepository<Diyetisyen> _diyetisyenRepository;

        public GetDiyetisyenUygunlukByTarihQueryHandler(
            IRepository<DiyetisyenUygunluk> repository,
            IRepository<Diyetisyen> diyetisyenRepository)
        {
            _repository = repository;
            _diyetisyenRepository = diyetisyenRepository;
        }

        public async Task<List<GetDiyetisyenUygunlukQueryResult>> Handle(GetDiyetisyenUygunlukByTarihQuery request, CancellationToken cancellationToken)
        {
            var uygunluklar = await _repository.GetAsync(u => 
                (u.BaslangicZamani >= request.BaslangicTarihi && u.BaslangicZamani <= request.BitisTarihi) || 
                (u.BitisZamani >= request.BaslangicTarihi && u.BitisZamani <= request.BitisTarihi) ||
                (u.BaslangicZamani <= request.BaslangicTarihi && u.BitisZamani >= request.BitisTarihi));

            var results = new List<GetDiyetisyenUygunlukQueryResult>();

            foreach (var uygunluk in uygunluklar)
            {
                var diyetisyen = await _diyetisyenRepository.GetByIdAsync(uygunluk.DiyetisyenId);

                var result = new GetDiyetisyenUygunlukQueryResult
                {
                    Id = uygunluk.Id,
                    DiyetisyenId = uygunluk.DiyetisyenId,
                    DiyetisyenAdSoyad = diyetisyen != null ? $"{diyetisyen.Ad} {diyetisyen.Soyad}" : "Bilinmiyor",
                    BaslangicZamani = uygunluk.BaslangicZamani,
                    BitisZamani = uygunluk.BitisZamani,
                    Muayit = uygunluk.Muayit,
                    TekrarlanirMi = uygunluk.TekrarlanirMi,
                    TekrarlanmaSikligi = uygunluk.TekrarlanmaSikligi
                };

                results.Add(result);
            }

            return results;
        }
    }
}