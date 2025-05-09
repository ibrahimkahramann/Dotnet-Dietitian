using Dotnet_Dietitian.Application.Features.CQRS.Queries.OdemeBilgisiQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Results.OdemeBilgisiResults;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.OdemeBilgisiHandlers
{
    public class GetOdemeBilgisiByIdQueryHandler : IRequestHandler<GetOdemeBilgisiByIdQuery, GetOdemeBilgisiByIdQueryResult>
    {
        private readonly IRepository<OdemeBilgisi> _repository;
        private readonly IRepository<Hasta> _hastaRepository;

        public GetOdemeBilgisiByIdQueryHandler(IRepository<OdemeBilgisi> repository, IRepository<Hasta> hastaRepository)
        {
            _repository = repository;
            _hastaRepository = hastaRepository;
        }

        public async Task<GetOdemeBilgisiByIdQueryResult> Handle(GetOdemeBilgisiByIdQuery request, CancellationToken cancellationToken)
        {
            var odeme = await _repository.GetByIdAsync(request.Id);
            if (odeme == null)
                throw new Exception($"ID:{request.Id} olan ödeme bilgisi bulunamadı");

            var hasta = await _hastaRepository.GetByIdAsync(odeme.HastaId);

            return new GetOdemeBilgisiByIdQueryResult
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
        }
    }
}