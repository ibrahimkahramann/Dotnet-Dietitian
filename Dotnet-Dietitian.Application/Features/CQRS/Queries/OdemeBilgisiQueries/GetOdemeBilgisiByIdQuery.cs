using Dotnet_Dietitian.Application.Features.CQRS.Results.OdemeBilgisiResults;
using MediatR;
using System;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.OdemeBilgisiQueries
{
    public class GetOdemeBilgisiByIdQuery : IRequest<GetOdemeBilgisiByIdQueryResult>
    {
        public Guid Id { get; set; }

        public GetOdemeBilgisiByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}