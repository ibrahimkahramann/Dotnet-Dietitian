using Dotnet_Dietitian.Application.Features.CQRS.Queries.OdemeBilgisiQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Results.OdemeBilgisiResults;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.OdemeBilgisiHandlers
{
    public class GetOdemeBilgisiQueryHandler : IRequestHandler<GetOdemeBilgisiQuery, List<GetOdemeBilgisiQueryResult>>
    {
        private readonly IRepository<OdemeBilgisi> _repository;
        private readonly IRepository<Hasta> _hastaRepository;

        public GetOdemeBilgisiQueryHandler(IRepository<OdemeBilgisi> repository, IRepository<Hasta> hastaRepository)
        {
            _repository = repository;
            _hastaRepository = hastaRepository;
        }

        public async Task<List<GetOdemeBilgisiQueryResult>> Handle(GetOdemeBilgisiQuery request, CancellationToken cancellationToken)
        {
            var odemeler = await _repository.GetAllAsync();
            var results = new List<GetOdemeBilgisiQueryResult>();

            foreach (var odeme in odemeler)
            {
                var hasta = await _hastaRepository.GetByIdAsync(odeme.HastaId);
                
                var result = new GetOdemeBilgisiQueryResult
                {
                    Id = odeme.Id,
                    HastaId = odeme.HastaId,
                    HastaAdSoyad = hasta != null ? $"{hasta.Ad} {hasta.Soyad}" : "Bilinmiyor",
                    Tutar = odeme.Tutar,
                    Tarih = odeme.Tarih,
                    OdemeTuru = odeme.OdemeTuru,
                    Aciklama = odeme.Aciklama,
                    IslemReferansNo = odeme.IslemReferansNo
                };

                results.Add(result);
            }

            return results;
        }
    }
}