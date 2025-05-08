using Dotnet_Dietitian.Application.Features.CQRS.Commands.RandevuCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.RandevuHandlers
{
    public class RemoveRandevuCommandHandler : IRequestHandler<RemoveRandevuCommand, Unit>
    {
        private readonly IRepository<Randevu> _repository;

        public RemoveRandevuCommandHandler(IRepository<Randevu> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(RemoveRandevuCommand request, CancellationToken cancellationToken)
        {
            var randevu = await _repository.GetByIdAsync(request.Id);
            if (randevu == null)
                throw new Exception($"ID:{request.Id} olan randevu bulunamadı");

            await _repository.DeleteAsync(randevu);
            return Unit.Value;
        }
    }
}