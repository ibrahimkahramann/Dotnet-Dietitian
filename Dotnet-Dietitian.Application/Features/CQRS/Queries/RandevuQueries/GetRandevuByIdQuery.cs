using Dotnet_Dietitian.Application.Features.CQRS.Results.RandevuResults;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.RandevuQueries
{
    public class GetRandevuByIdQuery : IRequest<GetRandevuByIdQueryResult>
    {
        public Guid Id { get; set; }

        public GetRandevuByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}