using Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetisyenUygunlukCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.DiyetisyenUygunlukHandlers
{
    public class UpdateDiyetisyenUygunlukCommandHandler : IRequestHandler<UpdateDiyetisyenUygunlukCommand, Unit>
    {
        private readonly IRepository<DiyetisyenUygunluk> _repository;

        public UpdateDiyetisyenUygunlukCommandHandler(IRepository<DiyetisyenUygunluk> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateDiyetisyenUygunlukCommand request, CancellationToken cancellationToken)
        {
            var uygunluk = await _repository.GetByIdAsync(request.Id);
            if (uygunluk == null)
                throw new Exception($"ID:{request.Id} olan diyetisyen uygunluğu bulunamadı");

            // Tarih kontrolü
            if (request.BaslangicZamani >= request.BitisZamani)
                throw new Exception("Başlangıç zamanı, bitiş zamanından önce olmalıdır");

            uygunluk.DiyetisyenId = request.DiyetisyenId;
            uygunluk.BaslangicZamani = request.BaslangicZamani;
            uygunluk.BitisZamani = request.BitisZamani;
            uygunluk.Musait = request.Musait;
            uygunluk.TekrarlanirMi = request.TekrarlanirMi;
            uygunluk.TekrarlanmaSikligi = request.TekrarlanmaSikligi;
            uygunluk.Notlar = request.Notlar;
            
            // Eski property'leri de güncelle
            uygunluk.Gun = request.BaslangicZamani.Date;
            uygunluk.BaslangicSaati = request.BaslangicZamani.TimeOfDay;
            uygunluk.BitisSaati = request.BitisZamani.TimeOfDay;
            uygunluk.TekrarTipi = request.TekrarlanmaSikligi;

            await _repository.UpdateAsync(uygunluk);
            return Unit.Value;
        }
    }
}