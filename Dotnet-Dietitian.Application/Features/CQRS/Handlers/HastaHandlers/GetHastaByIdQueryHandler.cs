using Dotnet_Dietitian.Application.Features.CQRS.Queries.HastaQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Results.HastaResults;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.HastaHandlers
{
    public class GetHastaByIdQueryHandler : IRequestHandler<GetHastaByIdQuery, GetHastaByIdQueryResult>
    {
        private readonly IRepository<Hasta> _repository;
        private readonly IRepository<Diyetisyen> _diyetisyenRepository;
        private readonly IRepository<DiyetProgrami> _diyetProgramiRepository;
        private readonly IRepository<OdemeBilgisi> _odemeRepository;
        private readonly IRepository<Randevu> _randevuRepository;

        public GetHastaByIdQueryHandler(
            IRepository<Hasta> repository, 
            IRepository<Diyetisyen> diyetisyenRepository, 
            IRepository<DiyetProgrami> diyetProgramiRepository,
            IRepository<OdemeBilgisi> odemeRepository,
            IRepository<Randevu> randevuRepository)
        {
            _repository = repository;
            _diyetisyenRepository = diyetisyenRepository;
            _diyetProgramiRepository = diyetProgramiRepository;
            _odemeRepository = odemeRepository;
            _randevuRepository = randevuRepository;
        }

        public async Task<GetHastaByIdQueryResult> Handle(GetHastaByIdQuery request, CancellationToken cancellationToken)
        {
            var hasta = await _repository.GetByIdAsync(request.Id);
            if (hasta == null)
                throw new Exception($"ID:{request.Id} olan hasta bulunamadı");

            var result = new GetHastaByIdQueryResult
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
                Odemeler = new List<OdemeDto>(),
                Randevular = new List<RandevuDto>()
            };

            // Diyetisyen adını getir
            if (hasta.DiyetisyenId.HasValue)
            {
                var diyetisyen = await _diyetisyenRepository.GetByIdAsync(hasta.DiyetisyenId.Value);
                if (diyetisyen != null)
                {
                    result.DiyetisyenAdi = $"{diyetisyen.Ad} {diyetisyen.Soyad}";
                }
            }

            // Diyet programı adını getir
            if (hasta.DiyetProgramiId.HasValue)
            {
                var diyetProgrami = await _diyetProgramiRepository.GetByIdAsync(hasta.DiyetProgramiId.Value);
                if (diyetProgrami != null)
                {
                    result.DiyetProgramiAdi = diyetProgrami.Ad;
                }
            }

            // Ödemeleri getir
            var odemeler = await _odemeRepository.GetAsync(o => o.HastaId == hasta.Id);
            if (odemeler.Any())
            {
                result.Odemeler = odemeler.Select(o => new OdemeDto
                {
                    Id = o.Id,
                    Tutar = o.Tutar,
                    Tarih = o.Tarih,
                    OdemeTuru = o.OdemeTuru
                }).ToList();
            }

            // Randevuları getir
            var randevular = await _randevuRepository.GetAsync(r => r.HastaId == hasta.Id);
            if (randevular.Any())
            {
                result.Randevular = randevular.Select(r => new RandevuDto
                {
                    Id = r.Id,
                    RandevuBaslangicTarihi = r.RandevuBaslangicTarihi,
                    RandevuBitisTarihi = r.RandevuBitisTarihi,
                    Durum = r.Durum
                }).ToList();
            }

            return result;
        }
    }
}