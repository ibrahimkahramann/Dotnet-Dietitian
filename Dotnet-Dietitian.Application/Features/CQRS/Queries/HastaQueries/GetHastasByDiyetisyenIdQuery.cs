using Dotnet_Dietitian.Application.Features.CQRS.Results.HastaResults;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.HastaQueries
{
    public class GetHastasByDiyetisyenIdQuery : IRequest<List<GetHastaQueryResult>>
    {
        public Guid DiyetisyenId { get; set; }

        public GetHastasByDiyetisyenIdQuery(Guid diyetisyenId)
        {
            DiyetisyenId = diyetisyenId;
        }
    }
}