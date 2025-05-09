using Dotnet_Dietitian.Application.Features.CQRS.Results.HastaResults;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.HastaQueries
{
    public class GetHastaQuery : IRequest<List<GetHastaQueryResult>>
    {
    }
}