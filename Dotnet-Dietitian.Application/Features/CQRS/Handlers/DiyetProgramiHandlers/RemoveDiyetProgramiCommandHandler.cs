using Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetProgramiCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.DiyetProgramiHandlers
{
    public class RemoveDiyetProgramiCommandHandler : IRequestHandler<RemoveDiyetProgramiCommand, Unit>
    {
        private readonly IRepository<DiyetProgrami> _repository;

        public RemoveDiyetProgramiCommandHandler(IRepository<DiyetProgrami> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(RemoveDiyetProgramiCommand request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.Id);
            if (value == null)
                throw new Exception("Silinecek diyet programı bulunamadı");

            await _repository.DeleteAsync(value);
            return Unit.Value;
        }
    }
}