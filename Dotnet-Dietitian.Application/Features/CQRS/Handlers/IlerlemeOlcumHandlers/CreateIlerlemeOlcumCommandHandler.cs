using System;
using System.Threading;
using System.Threading.Tasks;
using Dotnet_Dietitian.Application.Features.CQRS.Commands.IlerlemeOlcumCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.IlerlemeOlcumHandlers
{
    public class CreateIlerlemeOlcumCommandHandler : IRequestHandler<CreateIlerlemeOlcumCommand, Guid>
    {
        private readonly IRepository<IlerlemeOlcum> _repository;

        public CreateIlerlemeOlcumCommandHandler(IRepository<IlerlemeOlcum> repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateIlerlemeOlcumCommand request, CancellationToken cancellationToken)
        {
            var ilerlemeOlcum = new IlerlemeOlcum
            {
                Id = Guid.NewGuid(),
                HastaId = request.HastaId,
                OlcumTarihi = request.OlcumTarihi,
                Kilo = request.Kilo,
                BelCevresi = request.BelCevresi,
                KalcaCevresi = request.KalcaCevresi,
                GogusCevresi = request.GogusCevresi,
                KolCevresi = request.KolCevresi,
                VucutYagOrani = request.VucutYagOrani,
                Notlar = request.Notlar
            };

            await _repository.AddAsync(ilerlemeOlcum);
            return ilerlemeOlcum.Id;
        }
    }
} 