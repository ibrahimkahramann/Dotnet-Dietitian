using Dotnet_Dietitian.Application.Features.CQRS.Results.DiyetProgramiResults;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetProgramiQueries
{
    public class GetDiyetProgramiByIdQuery : IRequest<GetDiyetProgramiByIdQueryResult>
    {
        public Guid Id { get; set; }

        public GetDiyetProgramiByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}