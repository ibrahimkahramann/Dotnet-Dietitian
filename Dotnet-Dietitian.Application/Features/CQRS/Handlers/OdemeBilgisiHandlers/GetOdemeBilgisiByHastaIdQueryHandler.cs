using Dotnet_Dietitian.Application.Features.CQRS.Queries.OdemeBilgisiQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Results.OdemeBilgisiResults;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.OdemeBilgisiHandlers
{
    public class GetOdemeBilgisiByHastaIdQueryHandler : IRequestHandler<GetOdemeBilgisiByHastaIdQuery, List<GetOdemeBilgisiQueryResult>>
    {
        private readonly IRepository<OdemeBilgisi> _repository;
        private readonly IRepository<Hasta> _hastaRepository;

        public GetOdemeBilgisiByHastaIdQueryHandler(IRepository<OdemeBilgisi> repository, IRepository<Hasta> hastaRepository)
        {
            _repository = repository;
            _hastaRepository = hastaRepository;
        }

        public async Task<List<GetOdemeBilgisiQueryResult>> Handle(GetOdemeBilgisiByHastaIdQuery request, CancellationToken cancellationToken)
        {
            var odemeler = await _repository.GetAsync(o => o.HastaId == request.HastaId);
            var results = new List<GetOdemeBilgisiQueryResult>();
            
            var hasta = await _hastaRepository.GetByIdAsync(request.HastaId);
            var hastaAdSoyad = hasta != null ? $"{hasta.Ad} {hasta.Soyad}" : "Bilinmiyor";

            foreach (var odeme in odemeler)
            {
                var result = new GetOdemeBilgisiQueryResult
                {
                    Id = odeme.Id,
                    HastaId = odeme.HastaId,
                    HastaAdSoyad = hastaAdSoyad,
                    Tutar = odeme.Tutar,
                    Tarih = odeme.Tarih,
                    OdemeTuru = odeme.OdemeTuru,
                    Aciklama = odeme.Aciklama,
                    IslemReferansNo = odeme.IslemReferansNo
                };

                results.Add(result);
            }

            return results.OrderByDescending(r => r.Tarih).ToList();
        }
    }
}