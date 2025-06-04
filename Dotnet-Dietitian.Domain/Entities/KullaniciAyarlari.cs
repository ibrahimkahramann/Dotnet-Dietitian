namespace Dotnet_Dietitian.Domain.Entities;

public class KullaniciAyarlari : BaseEntity
{
    public Guid KullaniciId { get; set; }
    public string KullaniciTipi { get; set; } // "Hasta" veya "Diyetisyen"
    
    // Genel Ayarlar
    public string? Dil { get; set; } = "tr";
    public string? ZamanDilimi { get; set; } = "Europe/Istanbul";
    public string? TarihFormati { get; set; } = "dd/MM/yyyy";
    public string? OlcuBirimi { get; set; } = "metric"; // metric veya imperial
    
    // Diyetisyene özel çalışma saatleri
    public TimeSpan? CalismaBaslangicSaati { get; set; }
    public TimeSpan? CalismaBitisSaati { get; set; }
    public bool HaftaSonuCalisma { get; set; } = false;
    
    // Bildirim Ayarları
    public bool EmailRandevuBildirimleri { get; set; } = true;
    public bool EmailMesajBildirimleri { get; set; } = true;
    public bool EmailDiyetGuncellemeBildirimleri { get; set; } = true;
    public bool EmailPazarlamaBildirimleri { get; set; } = false;
    
    public bool UygulamaRandevuBildirimleri { get; set; } = true;
    public bool UygulamaMesajBildirimleri { get; set; } = true;
    public bool UygulamaDiyetGuncellemeBildirimleri { get; set; } = true;
    public bool UygulamaGunlukHatirlatmalar { get; set; } = true;
    
    // Diyetisyene özel bildirimler
    public bool EmailYeniHastaBildirimleri { get; set; } = true;
    public bool UygulamaYeniHastaBildirimleri { get; set; } = true;
    
    // Gizlilik Ayarları
    public bool YeniGirisUyarilari { get; set; } = true;
    public bool OturumZamanAsimi { get; set; } = true;
    public bool SaglikVerisiPaylasimiIzni { get; set; } = true;
    public bool AktiviteVerisiPaylasimiIzni { get; set; } = true;
    public bool AnonimKullanimVerisiPaylasimiIzni { get; set; } = false;
    
    // Diyetisyene özel gizlilik ayarları
    public bool ProfilGorunurlugu { get; set; } = true;
    
    // Görünüm Ayarları
    public string? Tema { get; set; } = "light"; // light, dark, system
    public string? PanelDuzeni { get; set; } = "default"; // default, compact, detailed
    public string? RenkSemasi { get; set; } = "blue"; // blue, green, purple, etc.
    
    // Hastaya özel görünüm tercihleri
    public bool IlerlemeGrafigiGoster { get; set; } = true;
    public bool SuTakibiGoster { get; set; } = true;
    public bool KaloriTakibiGoster { get; set; } = true;
    
    // Ayarların son güncellenme tarihi
    public DateTime SonGuncellemeTarihi { get; set; } = DateTime.Now;
} 