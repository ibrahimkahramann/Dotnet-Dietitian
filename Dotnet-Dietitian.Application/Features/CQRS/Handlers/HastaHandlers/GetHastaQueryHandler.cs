using Dotnet_Dietitian.Application.Features.CQRS.Queries.HastaQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Results.HastaResults;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.HastaHandlers
{
    public class GetHastaQueryHandler : IRequestHandler<GetHastaQuery, List<GetHastaQueryResult>>
    {
        private readonly IHastaRepository _repository;
        private readonly IRepository<Diyetisyen> _diyetisyenRepository;
        private readonly IRepository<DiyetProgrami> _diyetProgramiRepository;

        public GetHastaQueryHandler(IHastaRepository repository, IRepository<Diyetisyen> diyetisyenRepository, IRepository<DiyetProgrami> diyetProgramiRepository)
        {
            _repository = repository;
            _diyetisyenRepository = diyetisyenRepository;
            _diyetProgramiRepository = diyetProgramiRepository;
        }

        public async Task<List<GetHastaQueryResult>> Handle(GetHastaQuery request, CancellationToken cancellationToken)
        {
            var hastalar = await _repository.GetAllAsync();
            var results = new List<GetHastaQueryResult>();

            foreach (var hasta in hastalar)
            {
                var result = new GetHastaQueryResult
                {
                    Id = hasta.Id,
                    TcKimlikNumarasi = hasta.TcKimlikNumarasi,
                    Ad = hasta.Ad,
                    Soyad = hasta.Soyad,
                    Yas = hasta.Yas,
                    Boy = hasta.Boy,
                    Kilo = hasta.Kilo,
                    Email = hasta.Email,
                    Telefon = hasta.Telefon,
                    DiyetisyenId = hasta.DiyetisyenId,
                    DiyetProgramiId = hasta.DiyetProgramiId,
                    GunlukKaloriIhtiyaci = hasta.GunlukKaloriIhtiyaci
                };

                if (hasta.DiyetisyenId.HasValue)
                {
                    var diyetisyen = await _diyetisyenRepository.GetByIdAsync(hasta.DiyetisyenId.Value);
                    if (diyetisyen != null)
                    {
                        result.DiyetisyenAdi = $"{diyetisyen.Ad} {diyetisyen.Soyad}";
                    }
                }

                if (hasta.DiyetProgramiId.HasValue)
                {
                    var diyetProgrami = await _diyetProgramiRepository.GetByIdAsync(hasta.DiyetProgramiId.Value);
                    if (diyetProgrami != null)
                    {
                        result.DiyetProgramiAdi = diyetProgrami.Ad;
                    }
                }

                results.Add(result);
            }

            return results;
        }
    }
}