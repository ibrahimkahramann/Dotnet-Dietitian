using Dotnet_Dietitian.Application.Features.CQRS.Commands.OdemeBilgisiCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.OdemeBilgisiHandlers
{
    public class RemoveOdemeBilgisiCommandHandler : IRequestHandler<RemoveOdemeBilgisiCommand, Unit>
    {
        private readonly IRepository<OdemeBilgisi> _repository;

        public RemoveOdemeBilgisiCommandHandler(IRepository<OdemeBilgisi> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(RemoveOdemeBilgisiCommand request, CancellationToken cancellationToken)
        {
            var odeme = await _repository.GetByIdAsync(request.Id);
            if (odeme == null)
                throw new Exception($"ID:{request.Id} olan ödeme bilgisi bulunamadı");

            await _repository.DeleteAsync(odeme);
            return Unit.Value;
        }
    }
}