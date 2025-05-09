using Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetisyenUygunlukCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.DiyetisyenUygunlukHandlers
{
    public class UpdateMusaitDurumCommandHandler : IRequestHandler<UpdateMusaitDurumCommand, Unit>
    {
        private readonly IRepository<DiyetisyenUygunluk> _repository;

        public UpdateMusaitDurumCommandHandler(IRepository<DiyetisyenUygunluk> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateMusaitDurumCommand request, CancellationToken cancellationToken)
        {
            var uygunluk = await _repository.GetByIdAsync(request.Id);
            if (uygunluk == null)
                throw new Exception($"ID:{request.Id} olan diyetisyen uygunluğu bulunamadı");

            uygunluk.Musait = request.Musait; // Muayit -> Musait

            await _repository.UpdateAsync(uygunluk);
            return Unit.Value;
        }
    }
}