using Dotnet_Dietitian.Application.Features.CQRS.Results.HastaResults;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.HastaQueries
{
    public class GetHastaWithDiyetProgramiQuery : IRequest<GetHastaByIdQueryResult>
    {
        public Guid Id { get; set; }

        public GetHastaWithDiyetProgramiQuery(Guid id)
        {
            Id = id;
        }
    }
}