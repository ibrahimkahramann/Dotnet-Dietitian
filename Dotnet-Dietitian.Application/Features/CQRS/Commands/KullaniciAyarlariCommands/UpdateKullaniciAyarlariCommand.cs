using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.KullaniciAyarlariCommands
{
    public class UpdateKullaniciAyarlariCommand : IRequest<Unit>
    {
        public Guid KullaniciId { get; set; }
        public string KullaniciTipi { get; set; } // "Hasta" veya "Diyetisyen"
        public string AyarTipi { get; set; } // "general", "notifications", "privacy", "appearance"
        
        // Genel Ayarlar
        public string? Dil { get; set; }
        public string? ZamanDilimi { get; set; }
        public string? TarihFormati { get; set; }
        public string? OlcuBirimi { get; set; }
        
        // Diyetisyene özel çalışma saatleri
        public string? CalismaBaslangicSaati { get; set; }
        public string? CalismaBitisSaati { get; set; }
        public bool? HaftaSonuCalisma { get; set; }
        
        // Bildirim Ayarları
        public bool? EmailRandevuBildirimleri { get; set; }
        public bool? EmailMesajBildirimleri { get; set; }
        public bool? EmailDiyetGuncellemeBildirimleri { get; set; }
        public bool? EmailPazarlamaBildirimleri { get; set; }
        
        public bool? UygulamaRandevuBildirimleri { get; set; }
        public bool? UygulamaMesajBildirimleri { get; set; }
        public bool? UygulamaDiyetGuncellemeBildirimleri { get; set; }
        public bool? UygulamaGunlukHatirlatmalar { get; set; }
        
        // Diyetisyene özel bildirimler
        public bool? EmailYeniHastaBildirimleri { get; set; }
        public bool? UygulamaYeniHastaBildirimleri { get; set; }
        
        // Gizlilik Ayarları
        public bool? YeniGirisUyarilari { get; set; }
        public bool? OturumZamanAsimi { get; set; }
        public bool? SaglikVerisiPaylasimiIzni { get; set; }
        public bool? AktiviteVerisiPaylasimiIzni { get; set; }
        public bool? AnonimKullanimVerisiPaylasimiIzni { get; set; }
        
        // Diyetisyene özel gizlilik ayarları
        public bool? ProfilGorunurlugu { get; set; }
        
        // Görünüm Ayarları
        public string? Tema { get; set; }
        public string? PanelDuzeni { get; set; }
        public string? RenkSemasi { get; set; }
        
        // Hastaya özel görünüm tercihleri
        public bool? IlerlemeGrafigiGoster { get; set; }
        public bool? SuTakibiGoster { get; set; }
        public bool? KaloriTakibiGoster { get; set; }
    }
} 