using Dotnet_Dietitian.Application.Strategies;
using System;

namespace Dotnet_Dietitian.Application.Services
{
    public class KaloriHesaplamaService
    {
        private IKaloriHesaplamaStrategy _strategy;
        
        public KaloriHesaplamaService(IKaloriHesaplamaStrategy strategy)
        {
            _strategy = strategy;
        }
        
        public void SetStrategy(IKaloriHesaplamaStrategy strategy)
        {
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        }
        
        public int HesaplaGunlukKalori(float kilo, float boy, int yas, string cinsiyet, string aktiviteSeviyesi)
        {
            return _strategy.HesaplaGunlukKaloriIhtiyaci(kilo, boy, yas, cinsiyet, aktiviteSeviyesi);
        }
        
        public (decimal yagGram, decimal proteinGram, decimal karbonhidratGram) HesaplaMakroDagilimi(int gunlukKalori, string diyetTipi, float kilo)
        {
            var (yagKalori, proteinKalori, karbonhidratKalori) = _strategy.HesaplaMakroBesinDagilimi(gunlukKalori, diyetTipi);
            
            // Kalorileri gram cinsine çevir (1g yağ = 9 kalori, 1g protein/karbonhidrat = 4 kalori)
            decimal yagGram = yagKalori / 9;
            decimal proteinGram = proteinKalori / 4;
            decimal karbonhidratGram = karbonhidratKalori / 4;
            
            return (yagGram, proteinGram, karbonhidratGram);
        }
        
        // Kilo verme/alma için kalori ayarlaması
        public int AyarlaKaloriHedefi(int gunlukKalori, string hedef, float hedefKiloKaybi = 0)
        {
            return hedef.ToLower() switch
            {
                "kilo verme" => gunlukKalori - (int)(500 * hedefKiloKaybi), // Haftada 0.5 kg vermek için her 500 kalori eksik
                "kilo alma" => gunlukKalori + 500,  // Kilo almak için 500 kalori fazladan
                _ => gunlukKalori // Kilo koruma
            };
        }
    }
}