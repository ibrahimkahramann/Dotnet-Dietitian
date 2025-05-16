using Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetProgramiCommands;
using Dotnet_Dietitian.Application.Services;
using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.TemplatePattern
{
    public class AkdenizDiyetiProgram : AbstractDiyetProgramOlusturucu
    {
        public AkdenizDiyetiProgram(KaloriHesaplamaService kaloriHesaplamaService) 
            : base(kaloriHesaplamaService)
        {
        }
        
        protected override string OlusturProgramAdi()
        {
            return "Akdeniz Diyeti Programı";
        }
        
        protected override string OlusturProgramAciklama()
        {
            return "Kalp sağlığını destekleyen, zeytinyağı, kuruyemiş, sebze ve meyve ağırlıklı Akdeniz tipi beslenme programı.";
        }
        
        protected override int BelirleGunlukAdimHedefi()
        {
            return 7500; // Orta seviye aktivite
        }
        
        protected override string BelirleHedefTipi()
        {
            return "kilo koruma"; // Akdeniz diyeti genelde kilo koruma odaklıdır
        }
        
        protected override string BelirleOzelDiyetTipi()
        {
            return "dengeli"; // Dengeli makro dağılımı
        }
        
        protected override void OzelOnerilerEkle(CreateDiyetProgramiCommand program, Hasta hasta)
        {
            program.Aciklama += "\nÖneriler: Zeytinyağını ana yağ kaynağı olarak kullanın. " + 
                              "Haftada en az 2-3 kez balık tüketin. Kırmızı et tüketimini haftada 1-2 kez ile sınırlayın. " +
                              "Günlük 1-2 porsiyon kuruyemiş tüketin. İşlenmiş gıdalardan kaçının.";
        }
    }
}