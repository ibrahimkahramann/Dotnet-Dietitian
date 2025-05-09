using Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetProgramiQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Results.DiyetProgramiResults;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.DiyetProgramiHandlers
{
    public class GetDiyetProgramiQueryHandler : IRequestHandler<GetDiyetProgramiQuery, List<GetDiyetProgramiQueryResult>>
    {
        private readonly IRepository<DiyetProgrami> _repository;
        private readonly IRepository<Diyetisyen> _diyetisyenRepository;

        public GetDiyetProgramiQueryHandler(IRepository<DiyetProgrami> repository, IRepository<Diyetisyen> diyetisyenRepository)
        {
            _repository = repository;
            _diyetisyenRepository = diyetisyenRepository;
        }

        public async Task<List<GetDiyetProgramiQueryResult>> Handle(GetDiyetProgramiQuery request, CancellationToken cancellationToken)
        {
            var values = await _repository.GetAllAsync();
            var results = new List<GetDiyetProgramiQueryResult>();

            foreach (var item in values)
            {
                var result = new GetDiyetProgramiQueryResult
                {
                    Id = item.Id,
                    Ad = item.Ad,
                    Aciklama = item.Aciklama,
                    BaslangicTarihi = item.BaslangicTarihi,
                    BitisTarihi = item.BitisTarihi,
                    YagGram = item.YagGram,
                    ProteinGram = item.ProteinGram,
                    KarbonhidratGram = item.KarbonhidratGram,
                    GunlukAdimHedefi = item.GunlukAdimHedefi,
                    OlusturanDiyetisyenId = item.OlusturanDiyetisyenId
                };

                if (item.OlusturanDiyetisyenId.HasValue)
                {
                    var diyetisyen = await _diyetisyenRepository.GetByIdAsync(item.OlusturanDiyetisyenId.Value);
                    if (diyetisyen != null)
                    {
                        result.DiyetisyenAdi = $"{diyetisyen.Ad} {diyetisyen.Soyad}";
                    }
                }

                results.Add(result);
            }

            return results;
        }
    }
}