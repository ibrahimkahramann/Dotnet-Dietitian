using Dotnet_Dietitian.Application.Features.CQRS.Results.DiyetisyenResults;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetisyenQueries
{
    public class GetDiyetisyenBySehirQuery : IRequest<List<GetDiyetisyenQueryResult>>
    {
        public string Sehir { get; set; }

        public GetDiyetisyenBySehirQuery(string sehir)
        {
            Sehir = sehir;
        }
    }
}