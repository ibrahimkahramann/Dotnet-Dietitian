using System;

namespace Dotnet_Dietitian.Application.Features.CQRS.Results.OdemeBilgisiResults
{
    public class GetOdemeBilgisiByIdQueryResult
    {
        public Guid Id { get; set; }
        public Guid HastaId { get; set; }
        public string HastaAdSoyad { get; set; }
        public decimal Tutar { get; set; }
        public DateTime Tarih { get; set; }
        public string? OdemeTuru { get; set; }
        public string? Aciklama { get; set; }
        public string? IslemReferansNo { get; set; }
    }
}