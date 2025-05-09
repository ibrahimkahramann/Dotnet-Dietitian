using Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetProgramiQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Results.DiyetProgramiResults;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.DiyetProgramiHandlers
{
    public class GetDiyetProgramiByIdQueryHandler : IRequestHandler<GetDiyetProgramiByIdQuery, GetDiyetProgramiByIdQueryResult>
    {
        private readonly IDiyetProgramiRepository _repository;
        private readonly IRepository<Diyetisyen> _diyetisyenRepository;

        public GetDiyetProgramiByIdQueryHandler(IDiyetProgramiRepository repository, IRepository<Diyetisyen> diyetisyenRepository)
        {
            _repository = repository;
            _diyetisyenRepository = diyetisyenRepository;
        }

        public async Task<GetDiyetProgramiByIdQueryResult> Handle(GetDiyetProgramiByIdQuery request, CancellationToken cancellationToken)
        {
            var item = await _repository.GetDiyetProgramiWithHastalarAsync(request.Id);
            
            if (item == null)
                throw new Exception("Diyet programı bulunamadı");

            var result = new GetDiyetProgramiByIdQueryResult
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
                OlusturanDiyetisyenId = item.OlusturanDiyetisyenId,
                Hastalar = item.Hastalar?.Select(h => new HastaDto 
                { 
                    Id = h.Id, 
                    Ad = h.Ad, 
                    Soyad = h.Soyad 
                }).ToList()
            };

            if (item.OlusturanDiyetisyenId.HasValue)
            {
                var diyetisyen = await _diyetisyenRepository.GetByIdAsync(item.OlusturanDiyetisyenId.Value);
                if (diyetisyen != null)
                {
                    result.DiyetisyenAdi = $"{diyetisyen.Ad} {diyetisyen.Soyad}";
                }
            }

            return result;
        }
    }
}