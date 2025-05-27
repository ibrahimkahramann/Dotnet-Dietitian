using Dotnet_Dietitian.Application.Features.CQRS.Results.DiyetisyenResults;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetisyenQueries
{
    public class GetDiyetisyenWithHastalarQuery : IRequest<GetDiyetisyenByIdQueryResult>
    {
        public Guid Id { get; set; }

        public GetDiyetisyenWithHastalarQuery(Guid id)
        {
            Id = id;
        }
    }
} 