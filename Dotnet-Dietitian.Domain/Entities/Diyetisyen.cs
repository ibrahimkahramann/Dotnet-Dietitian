namespace Dotnet_Dietitian.Domain.Entities;

public class Diyetisyen : BaseEntity
{
    public string TcKimlikNumarasi { get; set; }
    public string Ad { get; set; }
    public string Soyad { get; set; }
    public string? Uzmanlik { get; set; }
    public string Email { get; set; }
    public string? Telefon { get; set; }
    public string? MezuniyetOkulu { get; set; }
    public int? DeneyimYili { get; set; }
    public float Puan { get; set; } = 0;
    public int ToplamYorumSayisi { get; set; } = 0;
    public string? Hakkinda { get; set; }
    public string? ProfilResmiUrl { get; set; }
    public string? Sehir { get; set; }
    
    // Navigation properties
    public virtual ICollection<Hasta> Hastalar { get; set; }
    public virtual ICollection<DiyetProgrami> OlusturulanProgramlar { get; set; }
    public virtual ICollection<Randevu> Randevular { get; set; }
    public virtual ICollection<DiyetisyenUygunluk> UygunlukZamanlari { get; set; }
}