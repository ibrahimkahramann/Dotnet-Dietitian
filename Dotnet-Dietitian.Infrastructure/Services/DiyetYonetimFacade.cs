using Dotnet_Dietitian.Application.Features.CQRS.Commands.OdemeBilgisiCommands;
using Dotnet_Dietitian.Application.Features.CQRS.Commands.RandevuCommands;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetProgramiQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.HastaQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.OdemeBilgisiQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.RandevuQueries;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Infrastructure.Services
{
    public class DiyetYonetimFacade : IDiyetYonetimFacade
    {
        private readonly IMediator _mediator;
        private readonly IRepository<Hasta> _hastaRepository;
        private readonly IRepository<Diyetisyen> _diyetisyenRepository;
        private readonly IRepository<DiyetProgrami> _diyetProgramiRepository;
        private readonly IRepository<Randevu> _randevuRepository;
        private readonly IRepository<OdemeBilgisi> _odemeBilgisiRepository;
        private readonly IAppConfigService _configService;

        public DiyetYonetimFacade(
            IMediator mediator,
            IRepository<Hasta> hastaRepository,
            IRepository<Diyetisyen> diyetisyenRepository,
            IRepository<DiyetProgrami> diyetProgramiRepository,
            IRepository<Randevu> randevuRepository,
            IRepository<OdemeBilgisi> odemeBilgisiRepository,
            IAppConfigService configService)
        {
            _mediator = mediator;
            _hastaRepository = hastaRepository;
            _diyetisyenRepository = diyetisyenRepository;
            _diyetProgramiRepository = diyetProgramiRepository;
            _randevuRepository = randevuRepository;
            _odemeBilgisiRepository = odemeBilgisiRepository;
            _configService = configService;
        }

        public async Task<Hasta> GetHastaWithDetailsAsync(Guid hastaId)
        {
            var result = await _mediator.Send(new GetHastaWithDiyetProgramiQuery(hastaId));
            return await _hastaRepository.GetByIdAsync(hastaId);
        }

        public async Task<bool> AtamaYapAsync(Guid hastaId, Guid diyetisyenId, Guid diyetProgramiId)
        {
            var hasta = await _hastaRepository.GetByIdAsync(hastaId);
            if (hasta == null) return false;

            var diyetisyen = await _diyetisyenRepository.GetByIdAsync(diyetisyenId);
            if (diyetisyen == null) return false;

            var diyetProgrami = await _diyetProgramiRepository.GetByIdAsync(diyetProgramiId);
            if (diyetProgrami == null) return false;

            hasta.DiyetisyenId = diyetisyenId;
            hasta.DiyetProgramiId = diyetProgramiId;

            await _hastaRepository.UpdateAsync(hasta);
            return true;
        }

        public async Task<bool> RandevuOlusturAsync(Guid hastaId, Guid diyetisyenId, DateTime baslangicZamani, TimeSpan sure, string notlar = null)
        {
            var command = new CreateRandevuCommand
            {
                HastaId = hastaId,
                DiyetisyenId = diyetisyenId,
                RandevuBaslangicTarihi = baslangicZamani,
                RandevuBitisTarihi = baslangicZamani.Add(sure),
                RandevuTuru = "Danışma",
                Notlar = notlar
            };

            await _mediator.Send(command);
            return true;
        }

        public async Task<IReadOnlyList<Randevu>> GetHastaGelecekRandevulariAsync(Guid hastaId)
        {
            var simdi = DateTime.Now;
            var randevular = await _randevuRepository.GetAsync(r => 
                r.HastaId == hastaId && 
                r.RandevuBaslangicTarihi > simdi);
                
            return randevular.OrderBy(r => r.RandevuBaslangicTarihi).ToList();
        }

        public async Task<DiyetProgrami> GetDiyetProgramiWithHastaDetailsAsync(Guid diyetProgramiId)
        {
            var diyetProgrami = await _diyetProgramiRepository.GetByIdAsync(diyetProgramiId);
            // İlişkili verileri manual olarak yükleme örneği (normalde repository'de Include kullanılabilir)
            return diyetProgrami;
        }

        public async Task<IReadOnlyList<DiyetProgrami>> GetDiyetisyenProgramlariAsync(Guid diyetisyenId)
        {
            return await _diyetProgramiRepository.GetAsync(dp => dp.OlusturanDiyetisyenId == diyetisyenId);
        }

        public async Task<bool> OdemeYapAsync(Guid hastaId, decimal tutar, string odemeTuru, string aciklama = null)
        {
            var command = new CreateOdemeBilgisiCommand
            {
                HastaId = hastaId,
                Tutar = tutar,
                Tarih = DateTime.Now,
                OdemeTuru = odemeTuru,
                Aciklama = aciklama ?? "Diyet programı ödemesi",
                IslemReferansNo = Guid.NewGuid().ToString().Substring(0, 8)
            };

            await _mediator.Send(command);
            return true;
        }

        public async Task<IReadOnlyList<OdemeBilgisi>> GetHastaOdemeleriAsync(Guid hastaId)
        {
            var odemeler = await _mediator.Send(new GetOdemeBilgisiByHastaIdQuery(hastaId));
            return await _odemeBilgisiRepository.GetAsync(o => o.HastaId == hastaId);
        }
    }
}