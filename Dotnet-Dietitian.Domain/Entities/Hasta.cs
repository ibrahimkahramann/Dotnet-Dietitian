namespace Dotnet_Dietitian.Domain.Entities;



public class Hasta : BaseEntity
{
    public string TcKimlikNumarasi { get; set; }
    public string Ad { get; set; }
    public string Soyad { get; set; }
    public int? Yas { get; set; }
    public float? Boy { get; set; }
    public float? Kilo { get; set; }
    public string Email { get; set; }
    public string? Telefon { get; set; }
    public Guid? DiyetisyenId { get; set; }
    public Guid? DiyetProgramiId { get; set; }
    public int? GunlukKaloriIhtiyaci { get; set; }

    // Navigation properties
    public virtual Diyetisyen? Diyetisyen { get; set; }
    public virtual DiyetProgrami? DiyetProgrami { get; set; }
    public virtual ICollection<OdemeBilgisi> Odemeler { get; set; }
    public virtual ICollection<Randevu> Randevular { get; set; }
    public virtual ICollection<Abonelik> Abonelikler { get; set; } // Yeni eklenen iliþki
}
