using Dotnet_Dietitian.Application.Features.CQRS.Commands.HastaCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.HastaHandlers
{
    public class UpdateHastaProfileCommandHandler : IRequestHandler<UpdateHastaProfileCommand, Unit>
    {
        private readonly IRepository<Hasta> _repository;

        public UpdateHastaProfileCommandHandler(IRepository<Hasta> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateHastaProfileCommand request, CancellationToken cancellationToken)
        {
            var hasta = await _repository.GetByIdAsync(request.Id);
            if (hasta == null)
                throw new Exception($"ID:{request.Id} olan hasta bulunamadı");

            // Update basic fields
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
            
            // Update additional profile fields
            hasta.DogumTarihi = request.DogumTarihi;
            hasta.Cinsiyet = request.Cinsiyet;
            hasta.Adres = request.Adres;
            hasta.KanGrubu = request.KanGrubu;
            hasta.Alerjiler = request.Alerjiler;
            hasta.KronikHastaliklar = request.KronikHastaliklar;
            hasta.KullanilanIlaclar = request.KullanilanIlaclar;
            hasta.SaglikBilgisiPaylasimiIzni = request.SaglikBilgisiPaylasimiIzni;

            await _repository.UpdateAsync(hasta);
            return Unit.Value;
        }
    }
} 