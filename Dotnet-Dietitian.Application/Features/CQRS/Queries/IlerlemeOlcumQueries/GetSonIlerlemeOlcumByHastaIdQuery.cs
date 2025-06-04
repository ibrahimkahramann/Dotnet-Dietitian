using System;
using Dotnet_Dietitian.Application.Features.CQRS.Results.IlerlemeOlcumResults;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.IlerlemeOlcumQueries
{
    public class GetSonIlerlemeOlcumByHastaIdQuery : IRequest<IlerlemeOlcumDto?>
    {
        public GetSonIlerlemeOlcumByHastaIdQuery(Guid hastaId)
        {
            HastaId = hastaId;
        }

        public Guid HastaId { get; set; }
    }
} 