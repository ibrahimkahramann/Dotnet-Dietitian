using Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetProgramiCommands;
using Dotnet_Dietitian.Application.Services;
using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.TemplatePattern
{
    public class KiloVermeProgram : AbstractDiyetProgramOlusturucu
    {
        public KiloVermeProgram(KaloriHesaplamaService kaloriHesaplamaService) 
            : base(kaloriHesaplamaService)
        {
        }
        
        protected override string OlusturProgramAdi()
        {
            return "Kilo Verme Programı";
        }
        
        protected override string OlusturProgramAciklama()
        {
            return "Sağlıklı şekilde kilo vermeyi hedefleyen hafif kalori açığı ve protein odaklı beslenme programı.";
        }
        
        protected override int BelirleGunlukAdimHedefi()
        {
            return 10000; // Kilo vermek için daha fazla adım
        }
        
        protected override string BelirleHedefTipi()
        {
            return "kilo verme";
        }
        
        protected override string BelirleOzelDiyetTipi()
        {
            return "yüksek protein";
        }
        
        protected override void OzelOnerilerEkle(CreateDiyetProgramiCommand program, Hasta hasta)
        {
            program.Aciklama += "\nÖneriler: Günde en az 2 litre su tüketin. Ana öğünlerde protein kaynağı bulundurun. " + 
                              "Karbonhidrat alımını günün ilk yarısında tamamlayın.";
        }
    }
}