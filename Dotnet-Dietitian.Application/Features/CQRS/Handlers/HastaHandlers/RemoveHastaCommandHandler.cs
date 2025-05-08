using Dotnet_Dietitian.Application.Features.CQRS.Commands.HastaCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.HastaHandlers
{
    public class RemoveHastaCommandHandler : IRequestHandler<RemoveHastaCommand, Unit>
    {
        private readonly IRepository<Hasta> _repository;

        public RemoveHastaCommandHandler(IRepository<Hasta> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(RemoveHastaCommand request, CancellationToken cancellationToken)
        {
            var hasta = await _repository.GetByIdAsync(request.Id);
            if (hasta == null)
                throw new Exception($"ID:{request.Id} olan hasta bulunamadı");

            await _repository.DeleteAsync(hasta);
            return Unit.Value;
        }
    }
}