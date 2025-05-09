using Dotnet_Dietitian.Application.Features.CQRS.Results.DiyetisyenUygunlukResults;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetisyenUygunlukQueries
{
    public class GetDiyetisyenUygunlukByIdQuery : IRequest<GetDiyetisyenUygunlukByIdQueryResult>
    {
        public Guid Id { get; set; }

        public GetDiyetisyenUygunlukByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}