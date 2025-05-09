namespace Dotnet_Dietitian.Application.Features.CQRS.Results.RandevuResults
{
    public class GetRandevuByIdQueryResult
    {
        public Guid Id { get; set; }
        public Guid HastaId { get; set; }
        public string HastaAdSoyad { get; set; }
        public string HastaEmail { get; set; }
        public string? HastaTelefon { get; set; }
        public Guid DiyetisyenId { get; set; }
        public string DiyetisyenAdSoyad { get; set; }
        public string? DiyetisyenUzmanlik { get; set; }
        public string DiyetisyenEmail { get; set; }
        public string? DiyetisyenTelefon { get; set; }
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