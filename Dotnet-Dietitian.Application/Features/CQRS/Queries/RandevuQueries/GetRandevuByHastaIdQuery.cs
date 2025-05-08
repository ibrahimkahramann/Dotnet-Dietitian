using Dotnet_Dietitian.Application.Features.CQRS.Results.RandevuResults;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.RandevuQueries
{
    public class GetRandevuByHastaIdQuery : IRequest<List<GetRandevuQueryResult>>
    {
        public Guid HastaId { get; set; }

        public GetRandevuByHastaIdQuery(Guid hastaId)
        {
            HastaId = hastaId;
        }
    }
}