namespace Dotnet_Dietitian.Application.Strategies
{
    public interface IKaloriHesaplamaStrategy
    {
        int HesaplaGunlukKaloriIhtiyaci(float kilo, float boy, int yas, string cinsiyet, string aktiviteSeviyesi);
        (decimal yag, decimal protein, decimal karbonhidrat) HesaplaMakroBesinDagilimi(int gunlukKalori, string diyetTipi);
    }
}