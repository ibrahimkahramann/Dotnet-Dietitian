using Dotnet_Dietitian.Application.Features.CQRS.Commands.HastaCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.HastaHandlers
{
    public class UpdateHastaCommandHandler : IRequestHandler<UpdateHastaCommand, Unit>
    {
        private readonly IRepository<Hasta> _repository;

        public UpdateHastaCommandHandler(IRepository<Hasta> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateHastaCommand request, CancellationToken cancellationToken)
        {
            var hasta = await _repository.GetByIdAsync(request.Id);
            if (hasta == null)
                throw new Exception($"ID:{request.Id} olan hasta bulunamadı");

            hasta.TcKimlikNumarasi = request.TcKimlikNumarasi;
            hasta.Ad = request.Ad;
            hasta.Soyad = request.Soyad;
            hasta.Yas = request.Yas;
            hasta.Boy = request.Boy;
            hasta.Kilo = request.Kilo;
            hasta.Email = request.Email;
            hasta.Telefon = request.Telefon;
            hasta.DiyetisyenId = request.DiyetisyenId;
            hasta.DiyetProgramiId = request.DiyetProgramiId;
            hasta.GunlukKaloriIhtiyaci = request.GunlukKaloriIhtiyaci;

            await _repository.UpdateAsync(hasta);
            return Unit.Value;
        }
    }
}