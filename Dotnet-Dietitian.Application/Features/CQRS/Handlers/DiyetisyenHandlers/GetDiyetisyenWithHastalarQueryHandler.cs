using Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetisyenQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Results.DiyetisyenResults;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.DiyetisyenHandlers
{
    public class GetDiyetisyenWithHastalarQueryHandler : IRequestHandler<GetDiyetisyenWithHastalarQuery, GetDiyetisyenByIdQueryResult>
    {
        private readonly IDiyetisyenRepository _repository;
        private readonly IRepository<Hasta> _hastaRepository;

        public GetDiyetisyenWithHastalarQueryHandler(IDiyetisyenRepository repository, IRepository<Hasta> hastaRepository)
        {
            _repository = repository;
            _hastaRepository = hastaRepository;
        }

        public async Task<GetDiyetisyenByIdQueryResult> Handle(GetDiyetisyenWithHastalarQuery request, CancellationToken cancellationToken)
        {
            var diyetisyen = await _repository.GetDiyetisyenWithHastalarAsync(request.Id);
            if (diyetisyen == null)
                throw new Exception($"ID:{request.Id} olan diyetisyen bulunamadı");

            var result = new GetDiyetisyenByIdQueryResult
            {
                Id = diyetisyen.Id,
                TcKimlikNumarasi = diyetisyen.TcKimlikNumarasi,
                Ad = diyetisyen.Ad,
                Soyad = diyetisyen.Soyad,
                Uzmanlik = diyetisyen.Uzmanlik,
                Email = diyetisyen.Email,
                Telefon = diyetisyen.Telefon,
                MezuniyetOkulu = diyetisyen.MezuniyetOkulu,
                DeneyimYili = diyetisyen.DeneyimYili,
                Puan = diyetisyen.Puan,
                ToplamYorumSayisi = diyetisyen.ToplamYorumSayisi,
                Hakkinda = diyetisyen.Hakkinda,
                ProfilResmiUrl = diyetisyen.ProfilResmiUrl,
                Sehir = diyetisyen.Sehir,
                KayitTarihi = diyetisyen.OlusturulmaTarihi,
                CalistigiKurum = diyetisyen.CalistigiKurum,
                Unvan = diyetisyen.Unvan,
                LisansNumarasi = diyetisyen.LisansNumarasi,
                Hastalar = new List<HastaDto>()
            };

            // Diyetisyenin hastalarını ekle
            if (diyetisyen.Hastalar != null && diyetisyen.Hastalar.Any())
            {
                result.Hastalar = diyetisyen.Hastalar.Select(h => new HastaDto
                {
                    Id = h.Id,
                    Ad = h.Ad,
                    Soyad = h.Soyad,
                    Email = h.Email
                }).ToList();
            }

            return result;
        }
    }
}
