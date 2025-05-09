using Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetisyenUygunlukQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Results.DiyetisyenUygunlukResults;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.DiyetisyenUygunlukHandlers
{
    public class GetDiyetisyenUygunlukByIdQueryHandler : IRequestHandler<GetDiyetisyenUygunlukByIdQuery, GetDiyetisyenUygunlukByIdQueryResult>
    {
        private readonly IRepository<DiyetisyenUygunluk> _repository;
        private readonly IRepository<Diyetisyen> _diyetisyenRepository;

        public GetDiyetisyenUygunlukByIdQueryHandler(
            IRepository<DiyetisyenUygunluk> repository,
            IRepository<Diyetisyen> diyetisyenRepository)
        {
            _repository = repository;
            _diyetisyenRepository = diyetisyenRepository;
        }

        public async Task<GetDiyetisyenUygunlukByIdQueryResult> Handle(GetDiyetisyenUygunlukByIdQuery request, CancellationToken cancellationToken)
        {
            var uygunluk = await _repository.GetByIdAsync(request.Id);
            if (uygunluk == null)
                throw new Exception($"ID:{request.Id} olan diyetisyen uygunluğu bulunamadı");

            var diyetisyen = await _diyetisyenRepository.GetByIdAsync(uygunluk.DiyetisyenId);
            if (diyetisyen == null)
                throw new Exception($"ID:{uygunluk.DiyetisyenId} olan diyetisyen bulunamadı");

            var result = new GetDiyetisyenUygunlukByIdQueryResult
            {
                Id = uygunluk.Id,
                DiyetisyenId = uygunluk.DiyetisyenId,
                DiyetisyenAdSoyad = $"{diyetisyen.Ad} {diyetisyen.Soyad}",
                DiyetisyenEmail = diyetisyen.Email,
                DiyetisyenTelefon = diyetisyen.Telefon,
                BaslangicZamani = uygunluk.BaslangicZamani,
                BitisZamani = uygunluk.BitisZamani,
                Musait = uygunluk.Musait,
                TekrarlanirMi = uygunluk.TekrarlanirMi,
                TekrarlanmaSikligi = uygunluk.TekrarlanmaSikligi,
                Notlar = uygunluk.Notlar,
                OlusturulmaTarihi = uygunluk.OlusturulmaTarihi
            };

            return result;
        }
    }
}