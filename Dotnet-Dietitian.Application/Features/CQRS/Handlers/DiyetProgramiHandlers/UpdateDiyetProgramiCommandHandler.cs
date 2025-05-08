using Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetProgramiCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.DiyetProgramiHandlers
{
    public class UpdateDiyetProgramiCommandHandler : IRequestHandler<UpdateDiyetProgramiCommand, Unit>
    {
        private readonly IRepository<DiyetProgrami> _repository;

        public UpdateDiyetProgramiCommandHandler(IRepository<DiyetProgrami> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateDiyetProgramiCommand request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.Id);
            if (value == null)
                throw new Exception("Güncellenecek diyet programı bulunamadı");

            value.Ad = request.Ad;
            value.Aciklama = request.Aciklama;
            value.BaslangicTarihi = request.BaslangicTarihi;
            value.BitisTarihi = request.BitisTarihi;
            value.YagGram = request.YagGram;
            value.ProteinGram = request.ProteinGram;
            value.KarbonhidratGram = request.KarbonhidratGram;
            value.GunlukAdimHedefi = request.GunlukAdimHedefi;
            value.OlusturanDiyetisyenId = request.OlusturanDiyetisyenId;

            await _repository.UpdateAsync(value);
            return Unit.Value;
        }
    }
}