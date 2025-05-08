using Dotnet_Dietitian.Application.Features.CQRS.Queries.HastaQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Results.HastaResults;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.HastaHandlers
{
    public class GetHastasByDiyetisyenIdQueryHandler : IRequestHandler<GetHastasByDiyetisyenIdQuery, List<GetHastaQueryResult>>
    {
        private readonly IHastaRepository _repository;
        private readonly IRepository<DiyetProgrami> _diyetProgramiRepository;

        public GetHastasByDiyetisyenIdQueryHandler(IHastaRepository repository, IRepository<DiyetProgrami> diyetProgramiRepository)
        {
            _repository = repository;
            _diyetProgramiRepository = diyetProgramiRepository;
        }

        public async Task<List<GetHastaQueryResult>> Handle(GetHastasByDiyetisyenIdQuery request, CancellationToken cancellationToken)
        {
            var hastalar = await _repository.GetHastasByDiyetisyenIdAsync(request.DiyetisyenId);
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
                    GunlukKaloriIhtiyaci = hasta.GunlukKaloriIhtiyaci,
                    DiyetisyenAdi = hasta.Diyetisyen != null ? $"{hasta.Diyetisyen.Ad} {hasta.Diyetisyen.Soyad}" : null
                };

                // Diyet programı adını getir
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