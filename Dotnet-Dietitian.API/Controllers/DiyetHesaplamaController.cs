using Dotnet_Dietitian.Application.Services;
using Dotnet_Dietitian.Application.Strategies;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Dotnet_Dietitian.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiyetHesaplamaController : ControllerBase
    {
        private readonly KaloriHesaplamaService _kaloriService;
        private readonly HarrisBenedictKaloriStrategy _harrisBenedictStrategy;
        private readonly MifflinStJeorKaloriStrategy _mifflinStJeorStrategy;

        public DiyetHesaplamaController(
            KaloriHesaplamaService kaloriService,
            HarrisBenedictKaloriStrategy harrisBenedictStrategy, 
            MifflinStJeorKaloriStrategy mifflinStJeorStrategy)
        {
            _kaloriService = kaloriService;
            _harrisBenedictStrategy = harrisBenedictStrategy;
            _mifflinStJeorStrategy = mifflinStJeorStrategy;
        }

        [HttpPost("hesapla")]
        public IActionResult HesaplaKalori(KaloriHesaplamaModel model)
        {
            // Stratejileri çalışma zamanında değiştirme örneği
            if (model.FormuTipi == "mifflin")
            {
                _kaloriService.SetStrategy(_mifflinStJeorStrategy);
            }
            else
            {
                _kaloriService.SetStrategy(_harrisBenedictStrategy);
            }
            
            var gunlukKalori = _kaloriService.HesaplaGunlukKalori(
                model.Kilo,
                model.Boy,
                model.Yas,
                model.Cinsiyet,
                model.AktiviteSeviyesi
            );
            
            // Hedef (kilo verme, alma veya koruma) bazlı kalori ayarlaması
            var hedefKalori = _kaloriService.AyarlaKaloriHedefi(gunlukKalori, model.Hedef, model.HedefKiloKaybi);
            
            // Makro besin dağılımını hesapla
            var makrolar = _kaloriService.HesaplaMakroDagilimi(hedefKalori, model.DiyetTipi, model.Kilo);
            
            var sonuc = new
            {
                BazalMetabolizmaKalorisi = gunlukKalori,
                HedefGunlukKalori = hedefKalori,
                MakroBesinler = new
                {
                    YagGram = Math.Round(makrolar.yagGram, 1),
                    ProteinGram = Math.Round(makrolar.proteinGram, 1),
                    KarbonhidratGram = Math.Round(makrolar.karbonhidratGram, 1)
                },
                KullandigiFormul = model.FormuTipi == "mifflin" ? "Mifflin-St Jeor" : "Harris-Benedict"
            };
            
            return Ok(sonuc);
        }
    }

    public class KaloriHesaplamaModel
    {
        public float Kilo { get; set; }
        public float Boy { get; set; }  // cm cinsinden
        public int Yas { get; set; }
        public string Cinsiyet { get; set; } // "Erkek" veya "Kadın"
        public string AktiviteSeviyesi { get; set; }
        public string DiyetTipi { get; set; }
        public string Hedef { get; set; } // "Kilo verme", "Kilo alma", "Kilo koruma"
        public float HedefKiloKaybi { get; set; } = 0.5f; // Haftalık hedef kg kaybı
        public string FormuTipi { get; set; } = "harris"; // "harris" veya "mifflin"
    }
}