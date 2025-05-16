using Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetProgramiCommands;
using Dotnet_Dietitian.Application.Services;
using Dotnet_Dietitian.Domain.Entities;
using System;

namespace Dotnet_Dietitian.Application.TemplatePattern
{
    public abstract class AbstractDiyetProgramOlusturucu
    {
        protected readonly KaloriHesaplamaService _kaloriHesaplamaService;
        
        public AbstractDiyetProgramOlusturucu(KaloriHesaplamaService kaloriHesaplamaService)
        {
            _kaloriHesaplamaService = kaloriHesaplamaService;
        }
        
        // Template Method - İş akışını tanımlar
        public CreateDiyetProgramiCommand ProgramOlustur(
            Hasta hasta, 
            Guid diyetisyenId, 
            int sureDays = 90,
            string aktiviteSeviyesi = "orta aktif")
        {
            // Kalori hesapla
            var gunlukKaloriIhtiyaci = HesaplaKalori(hasta, aktiviteSeviyesi);
            
            // Makroları hesapla
            var makrolar = HesaplaMakroBeslenme(gunlukKaloriIhtiyaci, hasta.Kilo ?? 70);
            
            // Program bilgilerini ayarla
            var program = new CreateDiyetProgramiCommand
            {
                Ad = OlusturProgramAdi(),
                Aciklama = OlusturProgramAciklama(),
                BaslangicTarihi = DateTime.Now,
                BitisTarihi = DateTime.Now.AddDays(sureDays),
                YagGram = makrolar.yagGram,
                ProteinGram = makrolar.proteinGram,
                KarbonhidratGram = makrolar.karbonhidratGram,
                GunlukAdimHedefi = BelirleGunlukAdimHedefi(),
                OlusturanDiyetisyenId = diyetisyenId
            };
            
            // Hook metot - opsiyonel özelleştirmeler için
            OzelOnerilerEkle(program, hasta);
            
            return program;
        }
        
        // Abstract metotlar (alt sınıfların uygulaması gerekir)
        protected abstract string OlusturProgramAdi();
        protected abstract string OlusturProgramAciklama();
        protected abstract int BelirleGunlukAdimHedefi();
        protected abstract string BelirleHedefTipi();
        protected abstract string BelirleOzelDiyetTipi();
        
        // Ortak implementasyon
        protected virtual int HesaplaKalori(Hasta hasta, string aktiviteSeviyesi)
        {
            if (hasta.Boy == null || hasta.Kilo == null || hasta.Yas == null)
                return 2000; // Varsayılan değer
                
            var cinsiyet = "Kadın"; // Varsayılan, gerçek sistemde kişi bilgilerinden alınmalı
            
            var gunlukKalori = _kaloriHesaplamaService.HesaplaGunlukKalori(
                hasta.Kilo.Value,
                hasta.Boy.Value,
                hasta.Yas.Value,
                cinsiyet,
                aktiviteSeviyesi
            );
            
            return _kaloriHesaplamaService.AyarlaKaloriHedefi(
                gunlukKalori, 
                BelirleHedefTipi()
            );
        }
        
        protected virtual (decimal yagGram, decimal proteinGram, decimal karbonhidratGram) HesaplaMakroBeslenme(
            int gunlukKalori, float kilo)
        {
            return _kaloriHesaplamaService.HesaplaMakroDagilimi(
                gunlukKalori, 
                BelirleOzelDiyetTipi(),
                kilo
            );
        }
        
        // Hook metot (opsiyonel olarak alt sınıflar override edebilir)
        protected virtual void OzelOnerilerEkle(CreateDiyetProgramiCommand program, Hasta hasta)
        {
            // Varsayılan implementasyon boş - alt sınıflar ihtiyaç duyarsa override edebilir
        }
    }
}