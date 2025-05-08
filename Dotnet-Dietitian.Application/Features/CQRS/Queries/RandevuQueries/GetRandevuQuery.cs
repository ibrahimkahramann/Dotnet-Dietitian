using Dotnet_Dietitian.Application.Features.CQRS.Results.RandevuResults;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.RandevuQueries
{
    public class GetRandevuQuery : IRequest<List<GetRandevuQueryResult>>
    {
    }
}