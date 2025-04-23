namespace Dotnet_Dietitian.Domain.Entities;

    public class DiyetProgrami : BaseEntity
    {
        public string Ad { get; set; }
        public string? Aciklama { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public decimal? YagGram { get; set; }
        public decimal? ProteinGram { get; set; }
        public decimal? KarbonhidratGram { get; set; }
        public int? GunlukAdimHedefi { get; set; }
        public Guid? OlusturanDiyetisyenId { get; set; }
        
        // Navigation properties
        public virtual Diyetisyen? OlusturanDiyetisyen { get; set; }
        public virtual ICollection<Hasta> Hastalar { get; set; }
    }
