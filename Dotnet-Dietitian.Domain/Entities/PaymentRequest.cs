namespace Dotnet_Dietitian.Domain.Entities;

public class PaymentRequest : BaseEntity
{
    public Guid HastaId { get; set; }
    public Guid DiyetisyenId { get; set; }
    public Guid DiyetProgramiId { get; set; }
    public decimal Tutar { get; set; }
    public DateTime? VadeTarihi { get; set; }
    public string? Aciklama { get; set; }
    public PaymentRequestStatus Durum { get; set; } = PaymentRequestStatus.Bekliyor;
    public DateTime? OdemeTarihi { get; set; }
    public Guid? OdemeBilgisiId { get; set; } // Ödeme yapıldığında ilişkili ödeme kaydının referansı
    public string? RedNotu { get; set; } // Ret durumunda açıklama
    
    // Navigation properties
    public virtual Hasta Hasta { get; set; }
    public virtual Diyetisyen Diyetisyen { get; set; }
    public virtual DiyetProgrami DiyetProgrami { get; set; }
    public virtual OdemeBilgisi? OdemeBilgisi { get; set; }
}

public enum PaymentRequestStatus
{
    Bekliyor = 0,      // Ödeme talebi oluşturuldu, hasta yanıtı bekleniyor
    Onaylandi = 1,     // Hasta tarafından onaylandı
    Reddedildi = 2,    // Hasta tarafından reddedildi
    Odendi = 3,        // Ödeme tamamlandı
    IptalEdildi = 4    // Diyetisyen tarafından iptal edildi
}
