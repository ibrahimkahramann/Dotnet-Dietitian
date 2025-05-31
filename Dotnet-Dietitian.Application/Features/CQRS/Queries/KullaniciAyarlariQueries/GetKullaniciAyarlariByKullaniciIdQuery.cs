using Dotnet_Dietitian.Application.Features.CQRS.Results.KullaniciAyarlariResults;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.KullaniciAyarlariQueries
{
    public class GetKullaniciAyarlariByKullaniciIdQuery : IRequest<GetKullaniciAyarlariQueryResult>
    {
        public Guid KullaniciId { get; set; }
        public string KullaniciTipi { get; set; } // "Hasta" veya "Diyetisyen"
        
        public GetKullaniciAyarlariByKullaniciIdQuery(Guid kullaniciId, string kullaniciTipi)
        {
            KullaniciId = kullaniciId;
            KullaniciTipi = kullaniciTipi;
        }
    }
} 