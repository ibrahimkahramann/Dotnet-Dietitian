using Dotnet_Dietitian.Application.Features.CQRS.Results.DiyetisyenUygunlukResults;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetisyenUygunlukQueries
{
    public class GetDiyetisyenUygunlukByDiyetisyenIdQuery : IRequest<List<GetDiyetisyenUygunlukQueryResult>>
    {
        public Guid DiyetisyenId { get; set; }

        public GetDiyetisyenUygunlukByDiyetisyenIdQuery(Guid diyetisyenId)
        {
            DiyetisyenId = diyetisyenId;
        }
    }
}