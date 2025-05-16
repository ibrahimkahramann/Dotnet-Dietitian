namespace Dotnet_Dietitian.Application.Strategies
{
    public class MifflinStJeorKaloriStrategy : IKaloriHesaplamaStrategy
    {
        public int HesaplaGunlukKaloriIhtiyaci(float kilo, float boy, int yas, string cinsiyet, string aktiviteSeviyesi)
        {
            // Mifflin-St Jeor formülü
            float bmh; // Bazal Metabolizma Hızı
            
            if (cinsiyet.ToLower() == "erkek") 
            {
                bmh = (10 * kilo) + (6.25f * boy) - (5 * yas) + 5;
            } 
            else 
            {
                bmh = (10 * kilo) + (6.25f * boy) - (5 * yas) - 161;
            }

            // Aktivite faktörü
            float aktiviteFaktoru = aktiviteSeviyesi.ToLower() switch 
            {
                "sedanter" => 1.2f,
                "hafif aktif" => 1.375f,
                "orta aktif" => 1.55f,
                "çok aktif" => 1.725f,
                "aşırı aktif" => 1.9f,
                _ => 1.2f
            };

            return (int)(bmh * aktiviteFaktoru);
        }

        public (decimal yag, decimal protein, decimal karbonhidrat) HesaplaMakroBesinDagilimi(int gunlukKalori, string diyetTipi)
        {
            // Diyetisyenin seçtiği diyet tipine göre makro besin dağılımı
            return diyetTipi.ToLower() switch 
            {
                "dengeli" => (gunlukKalori * 0.3m, gunlukKalori * 0.3m, gunlukKalori * 0.4m),
                "düşük karbonhidrat" => (gunlukKalori * 0.45m, gunlukKalori * 0.35m, gunlukKalori * 0.2m),
                "yüksek protein" => (gunlukKalori * 0.25m, gunlukKalori * 0.5m, gunlukKalori * 0.25m),
                "ketojenik" => (gunlukKalori * 0.7m, gunlukKalori * 0.25m, gunlukKalori * 0.05m),
                _ => (gunlukKalori * 0.3m, gunlukKalori * 0.3m, gunlukKalori * 0.4m)
            };
        }
    }
}