namespace Dotnet_Dietitian.Domain.Entities;

public class Randevu : BaseEntity
{
    public Guid HastaId { get; set; }
    public Guid DiyetisyenId { get; set; }
    public DateTime RandevuBaslangicTarihi { get; set; }
    public DateTime RandevuBitisTarihi { get; set; }
    public string? RandevuTuru { get; set; }
    public string Durum { get; set; } = "Bekliyor";
    public bool DiyetisyenOnayi { get; set; } = false;
    public bool HastaOnayi { get; set; } = false;
    public string? Notlar { get; set; }
    public DateTime YaratilmaTarihi { get; set; } = DateTime.Now;
    
    // Navigation properties
    public virtual Hasta Hasta { get; set; }
    public virtual Diyetisyen Diyetisyen { get; set; }
}
