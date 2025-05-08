namespace Dotnet_Dietitian.Application.Features.CQRS.Results.RandevuResults
{
    public class GetRandevuQueryResult
    {
        public Guid Id { get; set; }
        public Guid HastaId { get; set; }
        public string HastaAdSoyad { get; set; }
        public Guid DiyetisyenId { get; set; }
        public string DiyetisyenAdSoyad { get; set; }
        public DateTime RandevuBaslangicTarihi { get; set; }
        public DateTime RandevuBitisTarihi { get; set; }
        public string? RandevuTuru { get; set; }
        public string Durum { get; set; }
        public bool DiyetisyenOnayi { get; set; }
        public bool HastaOnayi { get; set; }
        public string? Notlar { get; set; }
        public DateTime YaratilmaTarihi { get; set; }
    }
}