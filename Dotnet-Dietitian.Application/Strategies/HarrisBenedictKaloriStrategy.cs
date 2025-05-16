namespace Dotnet_Dietitian.Application.Strategies
{
    public class HarrisBenedictKaloriStrategy : IKaloriHesaplamaStrategy
    {
        public int HesaplaGunlukKaloriIhtiyaci(float kilo, float boy, int yas, string cinsiyet, string aktiviteSeviyesi)
        {
            // Harris-Benedict formülü
            float bmh; // Bazal Metabolizma Hızı
            
            if (cinsiyet.ToLower() == "erkek") 
            {
                bmh = 88.362f + (13.397f * kilo) + (4.799f * boy) - (5.677f * yas);
            } 
            else 
            {
                bmh = 447.593f + (9.247f * kilo) + (3.098f * boy) - (4.330f * yas);
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
            return diyetTipi.ToLower() switch 
            {
                "dengeli" => (gunlukKalori * 0.3m, gunlukKalori * 0.3m, gunlukKalori * 0.4m),
                "düşük karbonhidrat" => (gunlukKalori * 0.4m, gunlukKalori * 0.4m, gunlukKalori * 0.2m),
                "yüksek protein" => (gunlukKalori * 0.25m, gunlukKalori * 0.45m, gunlukKalori * 0.3m),
                _ => (gunlukKalori * 0.3m, gunlukKalori * 0.3m, gunlukKalori * 0.4m)
            };
        }
    }
}