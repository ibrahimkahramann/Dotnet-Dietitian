using Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetProgramiCommands;
using Dotnet_Dietitian.Application.Services;
using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.TemplatePattern
{
    public class KasKazanmaProgram : AbstractDiyetProgramOlusturucu
    {
        public KasKazanmaProgram(KaloriHesaplamaService kaloriHesaplamaService) 
            : base(kaloriHesaplamaService)
        {
        }
        
        protected override string OlusturProgramAdi()
        {
            return "Kas Kazanma Programı";
        }
        
        protected override string OlusturProgramAciklama()
        {
            return "Kas kütlesini artırmaya odaklanan, protein ağırlıklı ve kalori fazlası içeren beslenme programı.";
        }
        
        protected override int BelirleGunlukAdimHedefi()
        {
            return 8000; // Kas kazanmak için orta seviye kardiyovasküler aktivite
        }
        
        protected override string BelirleHedefTipi()
        {
            return "kilo alma";
        }
        
        protected override string BelirleOzelDiyetTipi()
        {
            return "yüksek protein";
        }
        
        protected override void OzelOnerilerEkle(CreateDiyetProgramiCommand program, Hasta hasta)
        {
            program.Aciklama += "\nÖneriler: Antrenman sonrası 30 dakika içinde protein alımı sağlayın. " + 
                              "Günlük protein alımınızı en az 5-6 öğüne bölün. Antrenman öncesi kompleks karbonhidrat tüketin.";
        }
        
        protected override (decimal, decimal, decimal) HesaplaMakroBeslenme(int gunlukKalori, float kilo)
        {
            // Kas kazanma programında protein ihtiyacını vücut ağırlığına göre hesaplayalım
            // Vücut ağırlığının 2 katı protein (gram)
            decimal proteinGram = (decimal)kilo * 2;
            
            // Geriye kalan kalorileri yağ ve karbonhidrata bölelim
            decimal proteinKalori = proteinGram * 4; // 1 gram protein = 4 kalori
            decimal kalanKalori = gunlukKalori - (int)proteinKalori;
            
            // Kalan kalorilerin %40'ı yağ, %60'ı karbonhidrat olsun
            decimal yagKalori = kalanKalori * 0.4m;
            decimal karbonhidratKalori = kalanKalori * 0.6m;
            
            decimal yagGram = yagKalori / 9; // 1 gram yağ = 9 kalori
            decimal karbonhidratGram = karbonhidratKalori / 4; // 1 gram karbonhidrat = 4 kalori
            
            return (yagGram, proteinGram, karbonhidratGram);
        }
    }
}