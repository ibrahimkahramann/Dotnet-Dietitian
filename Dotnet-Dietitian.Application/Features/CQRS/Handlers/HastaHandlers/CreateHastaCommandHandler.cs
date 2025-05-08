using Dotnet_Dietitian.Application.Features.CQRS.Commands.HastaCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.HastaHandlers
{
    public class CreateHastaCommandHandler : IRequestHandler<CreateHastaCommand, Unit>
    {
        private readonly IRepository<Hasta> _repository;

        public CreateHastaCommandHandler(IRepository<Hasta> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(CreateHastaCommand request, CancellationToken cancellationToken)
        {
            await _repository.AddAsync(new Hasta
            {
                TcKimlikNumarasi = request.TcKimlikNumarasi,
                Ad = request.Ad,
                Soyad = request.Soyad,
                Yas = request.Yas,
                Boy = request.Boy,
                Kilo = request.Kilo,
                Email = request.Email,
                Telefon = request.Telefon,
                DiyetisyenId = request.DiyetisyenId,
                DiyetProgramiId = request.DiyetProgramiId,
                GunlukKaloriIhtiyaci = request.GunlukKaloriIhtiyaci
            });

            return Unit.Value;
        }
    }
}