namespace Dotnet_Dietitian.Domain.Entities;

public class DiyetisyenUygunluk : BaseEntity
{
    public Guid DiyetisyenId { get; set; }
    public DateTime BaslangicZamani { get; set; }
    public DateTime BitisZamani { get; set; }
    public bool Musait { get; set; } = true; // "Muayit" yerine "Musait" olarak düzeltildi
    public bool TekrarlanirMi { get; set; } = false;
    public string? TekrarlanmaSikligi { get; set; }
    public string? Notlar { get; set; }
    public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;
    
    // Eski property'ler için uyumluluk sağlayalım
    public DateTime Gun { get; set; }
    public TimeSpan BaslangicSaati { get; set; }
    public TimeSpan BitisSaati { get; set; }
    public string? TekrarTipi { get; set; }
    
    // Navigation properties
    public virtual Diyetisyen Diyetisyen { get; set; }
}
