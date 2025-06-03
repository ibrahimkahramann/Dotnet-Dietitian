using System.Collections.Generic;
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
    public class GetIlerlemeOlcumlerByHastaIdQueryHandler : IRequestHandler<GetIlerlemeOlcumlerByHastaIdQuery, List<IlerlemeOlcumDto>>
    {
        private readonly IRepository<IlerlemeOlcum> _repository;

        public GetIlerlemeOlcumlerByHastaIdQueryHandler(IRepository<IlerlemeOlcum> repository)
        {
            _repository = repository;
        }

        public async Task<List<IlerlemeOlcumDto>> Handle(GetIlerlemeOlcumlerByHastaIdQuery request, CancellationToken cancellationToken)
        {
            var ilerlemeOlcumler = await _repository.GetAllAsync();
            
            return ilerlemeOlcumler
                .Where(x => x.HastaId == request.HastaId)
                .OrderBy(x => x.OlcumTarihi)
                .Select(i => new IlerlemeOlcumDto
                {
                    Id = i.Id,
                    HastaId = i.HastaId,
                    OlcumTarihi = i.OlcumTarihi,
                    Kilo = i.Kilo,
                    BelCevresi = i.BelCevresi,
                    KalcaCevresi = i.KalcaCevresi,
                    GogusCevresi = i.GogusCevresi,
                    KolCevresi = i.KolCevresi,
                    VucutYagOrani = i.VucutYagOrani,
                    Notlar = i.Notlar
                })
                .ToList();
        }
    }
} 