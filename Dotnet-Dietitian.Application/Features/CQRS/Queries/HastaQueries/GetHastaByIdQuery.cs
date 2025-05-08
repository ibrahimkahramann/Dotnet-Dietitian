using Dotnet_Dietitian.Application.Features.CQRS.Results.HastaResults;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.HastaQueries
{
    public class GetHastaByIdQuery : IRequest<GetHastaByIdQueryResult>
    {
        public Guid Id { get; set; }

        public GetHastaByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}