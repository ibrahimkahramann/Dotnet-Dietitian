using Dotnet_Dietitian.Application.Features.CQRS.Results.DiyetProgramiResults;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetProgramiQueries
{
    public class GetDiyetProgramiQuery : IRequest<List<GetDiyetProgramiQueryResult>>
    {
        // Parametre gerektirmiyor
    }
}