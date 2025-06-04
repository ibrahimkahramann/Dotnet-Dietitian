using System;
using System.Collections.Generic;
using Dotnet_Dietitian.Application.Features.CQRS.Results.IlerlemeOlcumResults;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.IlerlemeOlcumQueries
{
    public class GetIlerlemeOlcumlerByHastaIdQuery : IRequest<List<IlerlemeOlcumDto>>
    {
        public GetIlerlemeOlcumlerByHastaIdQuery(Guid hastaId)
        {
            HastaId = hastaId;
        }

        public Guid HastaId { get; set; }
    }
} 