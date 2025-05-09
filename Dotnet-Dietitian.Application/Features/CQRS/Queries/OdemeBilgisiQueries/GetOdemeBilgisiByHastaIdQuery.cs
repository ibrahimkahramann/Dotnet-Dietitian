using Dotnet_Dietitian.Application.Features.CQRS.Results.OdemeBilgisiResults;
using MediatR;
using System;
using System.Collections.Generic;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.OdemeBilgisiQueries
{
    public class GetOdemeBilgisiByHastaIdQuery : IRequest<List<GetOdemeBilgisiQueryResult>>
    {
        public Guid HastaId { get; set; }

        public GetOdemeBilgisiByHastaIdQuery(Guid hastaId)
        {
            HastaId = hastaId;
        }
    }
}