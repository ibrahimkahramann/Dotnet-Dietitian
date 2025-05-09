namespace Dotnet_Dietitian.Application.Features.CQRS.Results.DiyetProgramiResults
{
    public class GetDiyetProgramiByIdQueryResult
    {
        public Guid Id { get; set; }
        public string Ad { get; set; }
        public string? Aciklama { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public decimal? YagGram { get; set; }
        public decimal? ProteinGram { get; set; }
        public decimal? KarbonhidratGram { get; set; }
        public int? GunlukAdimHedefi { get; set; }
        public Guid? OlusturanDiyetisyenId { get; set; }
        public string? DiyetisyenAdi { get; set; }
        public List<HastaDto>? Hastalar { get; set; }
    }

    public class HastaDto
    {
        public Guid Id { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
    }
}