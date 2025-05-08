using Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetProgramiCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.DiyetProgramiHandlers
{
    public class CreateDiyetProgramiCommandHandler : IRequestHandler<CreateDiyetProgramiCommand, Unit>
    {
        private readonly IRepository<DiyetProgrami> _repository;

        public CreateDiyetProgramiCommandHandler(IRepository<DiyetProgrami> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(CreateDiyetProgramiCommand request, CancellationToken cancellationToken)
        {
            await _repository.AddAsync(new DiyetProgrami
            {
                Ad = request.Ad,
                Aciklama = request.Aciklama,
                BaslangicTarihi = request.BaslangicTarihi,
                BitisTarihi = request.BitisTarihi,
                YagGram = request.YagGram,
                ProteinGram = request.ProteinGram,
                KarbonhidratGram = request.KarbonhidratGram,
                GunlukAdimHedefi = request.GunlukAdimHedefi,
                OlusturanDiyetisyenId = request.OlusturanDiyetisyenId
            });

            return Unit.Value;
        }
    }
}