using Dotnet_Dietitian.Application.Features.CQRS.Results.RandevuResults;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.RandevuQueries
{
    public class GetRandevuByDiyetisyenIdQuery : IRequest<List<GetRandevuQueryResult>>
    {
        public Guid DiyetisyenId { get; set; }

        public GetRandevuByDiyetisyenIdQuery(Guid diyetisyenId)
        {
            DiyetisyenId = diyetisyenId;
        }
    }
}