namespace Dotnet_Dietitian.Application.Features.CQRS.Results.HastaResults
{
    public class GetHastaByIdQueryResult
    {
        public Guid Id { get; set; }
        public string TcKimlikNumarasi { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public int? Yas { get; set; }
        public float? Boy { get; set; }
        public float? Kilo { get; set; }
        public string Email { get; set; }
        public string? Telefon { get; set; }
        public Guid? DiyetisyenId { get; set; }
        public string? DiyetisyenAdi { get; set; }
        public Guid? DiyetProgramiId { get; set; }
        public string? DiyetProgramiAdi { get; set; }
        public int? GunlukKaloriIhtiyaci { get; set; }
        public List<OdemeDto>? Odemeler { get; set; }
        public List<RandevuDto>? Randevular { get; set; }
    }

    public class OdemeDto
    {
        public Guid Id { get; set; }
        public decimal Tutar { get; set; }
        public DateTime Tarih { get; set; }
        public string OdemeTuru { get; set; }
    }

    public class RandevuDto
    {
        public Guid Id { get; set; }
        public DateTime RandevuBaslangicTarihi { get; set; }
        public DateTime RandevuBitisTarihi { get; set; }
        public string? RandevuTuru { get; set; }
        public string? Durum { get; set; }
    }
}