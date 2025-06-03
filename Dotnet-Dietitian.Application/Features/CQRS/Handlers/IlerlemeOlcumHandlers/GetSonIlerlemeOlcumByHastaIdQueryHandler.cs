using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.IlerlemeOlcumQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Results.IlerlemeOlcumResults;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.IlerlemeOlcumHandlers
{
    public class GetSonIlerlemeOlcumByHastaIdQueryHandler : IRequestHandler<GetSonIlerlemeOlcumByHastaIdQuery, IlerlemeOlcumDto?>
    {
        private readonly IRepository<IlerlemeOlcum> _repository;

        public GetSonIlerlemeOlcumByHastaIdQueryHandler(IRepository<IlerlemeOlcum> repository)
        {
            _repository = repository;
        }

        public async Task<IlerlemeOlcumDto?> Handle(GetSonIlerlemeOlcumByHastaIdQuery request, CancellationToken cancellationToken)
        {
            var ilerlemeOlcumler = await _repository.GetAllAsync();
            
            var sonOlcum = ilerlemeOlcumler
                .Where(x => x.HastaId == request.HastaId)
                .OrderByDescending(x => x.OlcumTarihi)
                .FirstOrDefault();

            if (sonOlcum == null)
                return null;

            return new IlerlemeOlcumDto
            {
                Id = sonOlcum.Id,
                HastaId = sonOlcum.HastaId,
                OlcumTarihi = sonOlcum.OlcumTarihi,
                Kilo = sonOlcum.Kilo,
                BelCevresi = sonOlcum.BelCevresi,
                KalcaCevresi = sonOlcum.KalcaCevresi,
                GogusCevresi = sonOlcum.GogusCevresi,
                KolCevresi = sonOlcum.KolCevresi,
                VucutYagOrani = sonOlcum.VucutYagOrani,
                Notlar = sonOlcum.Notlar
            };
        }
    }
} 