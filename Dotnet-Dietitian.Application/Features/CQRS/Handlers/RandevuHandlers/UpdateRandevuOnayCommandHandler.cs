using Dotnet_Dietitian.Application.Features.CQRS.Commands.RandevuCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.RandevuHandlers
{
    public class UpdateRandevuOnayCommandHandler : IRequestHandler<UpdateRandevuOnayCommand, Unit>
    {
        private readonly IRepository<Randevu> _repository;

        public UpdateRandevuOnayCommandHandler(IRepository<Randevu> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateRandevuOnayCommand request, CancellationToken cancellationToken)
        {
            var randevu = await _repository.GetByIdAsync(request.Id);
            if (randevu == null)
                throw new Exception($"ID:{request.Id} olan randevu bulunamadı");

            randevu.DiyetisyenOnayi = request.DiyetisyenOnayi;
            randevu.HastaOnayi = request.HastaOnayi;
            randevu.Durum = request.Durum;

            await _repository.UpdateAsync(randevu);
            return Unit.Value;
        }
    }
}