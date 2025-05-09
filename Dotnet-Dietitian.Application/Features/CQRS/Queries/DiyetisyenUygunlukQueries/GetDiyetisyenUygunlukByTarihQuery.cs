using Dotnet_Dietitian.Application.Features.CQRS.Results.DiyetisyenUygunlukResults;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetisyenUygunlukQueries
{
    public class GetDiyetisyenUygunlukByTarihQuery : IRequest<List<GetDiyetisyenUygunlukQueryResult>>
    {
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }

        public GetDiyetisyenUygunlukByTarihQuery(DateTime baslangicTarihi, DateTime bitisTarihi)
        {
            BaslangicTarihi = baslangicTarihi;
            BitisTarihi = bitisTarihi;
        }
    }
}