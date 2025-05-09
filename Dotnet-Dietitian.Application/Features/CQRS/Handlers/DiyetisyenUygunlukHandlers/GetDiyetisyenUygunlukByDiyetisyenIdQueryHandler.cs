using Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetisyenUygunlukQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Results.DiyetisyenUygunlukResults;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.DiyetisyenUygunlukHandlers
{
    public class GetDiyetisyenUygunlukByDiyetisyenIdQueryHandler : IRequestHandler<GetDiyetisyenUygunlukByDiyetisyenIdQuery, List<GetDiyetisyenUygunlukQueryResult>>
    {
        private readonly IRepository<DiyetisyenUygunluk> _repository;
        private readonly IRepository<Diyetisyen> _diyetisyenRepository;

        public GetDiyetisyenUygunlukByDiyetisyenIdQueryHandler(
            IRepository<DiyetisyenUygunluk> repository,
            IRepository<Diyetisyen> diyetisyenRepository)
        {
            _repository = repository;
            _diyetisyenRepository = diyetisyenRepository;
        }

        public async Task<List<GetDiyetisyenUygunlukQueryResult>> Handle(GetDiyetisyenUygunlukByDiyetisyenIdQuery request, CancellationToken cancellationToken)
        {
            var diyetisyen = await _diyetisyenRepository.GetByIdAsync(request.DiyetisyenId);
            if (diyetisyen == null)
                throw new Exception($"ID:{request.DiyetisyenId} olan diyetisyen bulunamadı");

            var uygunluklar = await _repository.GetAsync(u => u.DiyetisyenId == request.DiyetisyenId);
            var results = uygunluklar.Select(uygunluk => new GetDiyetisyenUygunlukQueryResult
            {
                Id = uygunluk.Id,
                DiyetisyenId = uygunluk.DiyetisyenId,
                DiyetisyenAdSoyad = $"{diyetisyen.Ad} {diyetisyen.Soyad}",
                BaslangicZamani = uygunluk.BaslangicZamani,
                BitisZamani = uygunluk.BitisZamani,
                Musait = uygunluk.Musait,
                TekrarlanirMi = uygunluk.TekrarlanirMi,
                TekrarlanmaSikligi = uygunluk.TekrarlanmaSikligi
            }).ToList();

            return results;
        }
    }
}