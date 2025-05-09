using Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetisyenUygunlukCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.DiyetisyenUygunlukHandlers
{
    public class CreateDiyetisyenUygunlukCommandHandler : IRequestHandler<CreateDiyetisyenUygunlukCommand, Unit>
    {
        private readonly IRepository<DiyetisyenUygunluk> _repository;
        private readonly IRepository<Diyetisyen> _diyetisyenRepository;

        public CreateDiyetisyenUygunlukCommandHandler(
            IRepository<DiyetisyenUygunluk> repository,
            IRepository<Diyetisyen> diyetisyenRepository)
        {
            _repository = repository;
            _diyetisyenRepository = diyetisyenRepository;
        }

        public async Task<Unit> Handle(CreateDiyetisyenUygunlukCommand request, CancellationToken cancellationToken)
        {
            // Diyetisyen var mı kontrolü
            var diyetisyen = await _diyetisyenRepository.GetByIdAsync(request.DiyetisyenId);
            if (diyetisyen == null)
                throw new Exception($"ID:{request.DiyetisyenId} olan diyetisyen bulunamadı");

            // Tarih kontrolü
            if (request.BaslangicZamani >= request.BitisZamani)
                throw new Exception("Başlangıç zamanı, bitiş zamanından önce olmalıdır");

            await _repository.AddAsync(new DiyetisyenUygunluk
            {
                DiyetisyenId = request.DiyetisyenId,
                BaslangicZamani = request.BaslangicZamani,
                BitisZamani = request.BitisZamani,
                Musait = request.Musait,
                TekrarlanirMi = request.TekrarlanirMi,
                TekrarlanmaSikligi = request.TekrarlanmaSikligi,
                Notlar = request.Notlar,
                OlusturulmaTarihi = request.OlusturulmaTarihi,
                // Eski property'ler için de değer ata
                Gun = request.BaslangicZamani.Date, 
                BaslangicSaati = request.BaslangicZamani.TimeOfDay,
                BitisSaati = request.BitisZamani.TimeOfDay,
                TekrarTipi = request.TekrarlanmaSikligi
            });

            return Unit.Value;
        }
    }
}