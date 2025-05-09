using Dotnet_Dietitian.Application.Features.CQRS.Results.DiyetisyenUygunlukResults;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetisyenUygunlukQueries
{
    public class GetDiyetisyenUygunlukQuery : IRequest<List<GetDiyetisyenUygunlukQueryResult>>
    {
    }
}