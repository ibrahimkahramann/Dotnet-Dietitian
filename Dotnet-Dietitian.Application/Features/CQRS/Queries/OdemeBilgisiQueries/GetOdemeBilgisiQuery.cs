using Dotnet_Dietitian.Application.Features.CQRS.Results.OdemeBilgisiResults;
using MediatR;
using System.Collections.Generic;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.OdemeBilgisiQueries
{
    public class GetOdemeBilgisiQuery : IRequest<List<GetOdemeBilgisiQueryResult>>
    {
        // Parametre gerektirmiyor
    }
}