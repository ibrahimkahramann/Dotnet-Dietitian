using Dotnet_Dietitian.Application.Features.CQRS.Results.DiyetisyenResults;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetisyenQueries
{
    public class GetDiyetisyenByIdQuery : IRequest<GetDiyetisyenByIdQueryResult>
    {
        public Guid Id { get; set; }

        public GetDiyetisyenByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}