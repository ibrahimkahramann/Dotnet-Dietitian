using Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetisyenQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Results.DiyetisyenResults;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.DiyetisyenHandlers
{
    public class GetDiyetisyenQueryHandler : IRequestHandler<GetDiyetisyenQuery, List<GetDiyetisyenQueryResult>>
    {
        private readonly IDiyetisyenRepository _repository;

        public GetDiyetisyenQueryHandler(IDiyetisyenRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<GetDiyetisyenQueryResult>> Handle(GetDiyetisyenQuery request, CancellationToken cancellationToken)
        {
            var values = await _repository.GetAllAsync();
            var results = values.Select(d => new GetDiyetisyenQueryResult
            {
                Id = d.Id,
                TcKimlikNumarasi = d.TcKimlikNumarasi,
                Ad = d.Ad,
                Soyad = d.Soyad,
                Uzmanlik = d.Uzmanlik,
                Email = d.Email,
                Telefon = d.Telefon,
                MezuniyetOkulu = d.MezuniyetOkulu,
                DeneyimYili = d.DeneyimYili,
                Puan = d.Puan,
                ToplamYorumSayisi = d.ToplamYorumSayisi,
                Hakkinda = d.Hakkinda,
                ProfilResmiUrl = d.ProfilResmiUrl,
                Sehir = d.Sehir,
                HastaSayisi = d.Hastalar?.Count ?? 0
            }).ToList();

            return results;
        }
    }
}