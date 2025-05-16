using System;

namespace Dotnet_Dietitian.Application.Features.CQRS.Results.MesajResults
{
    public class GetMesajQueryResult
    {
        public Guid Id { get; set; }
        public Guid GonderenId { get; set; }
        public string GonderenTipi { get; set; }
        public string GonderenAd { get; set; }
        public Guid AliciId { get; set; }
        public string AliciTipi { get; set; }
        public string AliciAd { get; set; }
        public string Icerik { get; set; }
        public DateTime GonderimZamani { get; set; }
        public bool Okundu { get; set; }
        public DateTime? OkunmaZamani { get; set; }
    }
}