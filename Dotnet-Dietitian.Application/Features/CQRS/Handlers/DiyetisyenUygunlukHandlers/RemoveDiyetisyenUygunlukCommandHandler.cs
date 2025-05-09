using Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetisyenUygunlukCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.DiyetisyenUygunlukHandlers
{
    public class RemoveDiyetisyenUygunlukCommandHandler : IRequestHandler<RemoveDiyetisyenUygunlukCommand, Unit>
    {
        private readonly IRepository<DiyetisyenUygunluk> _repository;

        public RemoveDiyetisyenUygunlukCommandHandler(IRepository<DiyetisyenUygunluk> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(RemoveDiyetisyenUygunlukCommand request, CancellationToken cancellationToken)
        {
            var uygunluk = await _repository.GetByIdAsync(request.Id);
            if (uygunluk == null)
                throw new Exception($"ID:{request.Id} olan diyetisyen uygunluğu bulunamadı");

            await _repository.DeleteAsync(uygunluk);
            return Unit.Value;
        }
    }
}