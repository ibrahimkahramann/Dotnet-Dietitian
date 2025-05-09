using Dotnet_Dietitian.Application.Features.CQRS.Results.DiyetisyenResults;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetisyenQueries
{
    public class GetDiyetisyenQuery : IRequest<List<GetDiyetisyenQueryResult>>
    {
    }
}