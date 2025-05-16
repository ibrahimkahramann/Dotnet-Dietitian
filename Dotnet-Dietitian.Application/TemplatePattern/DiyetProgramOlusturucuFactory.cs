using Dotnet_Dietitian.Application.Services;
using System;

namespace Dotnet_Dietitian.Application.TemplatePattern
{
    public class DiyetProgramOlusturucuFactory
    {
        private readonly KaloriHesaplamaService _kaloriHesaplamaService;
        
        public DiyetProgramOlusturucuFactory(KaloriHesaplamaService kaloriHesaplamaService)
        {
            _kaloriHesaplamaService = kaloriHesaplamaService;
        }
        
        public AbstractDiyetProgramOlusturucu GetProgramOlusturucu(string programTipi)
        {
            return programTipi?.ToLower() switch
            {
                "kilo verme" => new KiloVermeProgram(_kaloriHesaplamaService),
                "kas kazanma" => new KasKazanmaProgram(_kaloriHesaplamaService),
                "akdeniz diyeti" => new AkdenizDiyetiProgram(_kaloriHesaplamaService),
                _ => throw new ArgumentException($"Geçersiz program tipi: {programTipi}")
            };
        }
    }
}