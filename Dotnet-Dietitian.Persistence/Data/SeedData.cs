using Dotnet_Dietitian.Domain.Entities;
using Dotnet_Dietitian.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Persistence.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            // Diyetisyenler
            if (!context.Diyetisyenler.Any())
            {
                var diyetisyenler = new List<Diyetisyen>
                {
                    new Diyetisyen
                    {
                        Id = Guid.NewGuid(),
                        TcKimlikNumarasi = "12345678901",
                        Ad = "Ahmet",
                        Soyad = "Yılmaz",
                        Uzmanlik = "Sporcu Beslenmesi",
                        Email = "ahmet.yilmaz@example.com",
                        Telefon = "5551234567",
                        MezuniyetOkulu = "Ankara Üniversitesi",
                        DeneyimYili = 5,
                        Puan = 4.5f,
                        ToplamYorumSayisi = 20,
                        Hakkinda = "Sporcu beslenmesi konusunda uzmanlaşmış bir diyetisyenim.",
                        Sehir = "Ankara"
                    },
                    new Diyetisyen
                    {
                        Id = Guid.NewGuid(),
                        TcKimlikNumarasi = "23456789012",
                        Ad = "Ayşe",
                        Soyad = "Demir",
                        Uzmanlik = "Çocuk Beslenmesi",
                        Email = "ayse.demir@example.com",
                        Telefon = "5559876543",
                        MezuniyetOkulu = "İstanbul Üniversitesi",
                        DeneyimYili = 8,
                        Puan = 4.8f,
                        ToplamYorumSayisi = 35,
                        Hakkinda = "Çocuk ve adölesan beslenmesi üzerine çalışmaktayım.",
                        Sehir = "İstanbul"
                    }
                };
                
                await context.Diyetisyenler.AddRangeAsync(diyetisyenler);
                await context.SaveChangesAsync();
            }
            
            // Diyet Programları
            if (!context.DiyetProgramlari.Any())
            {
                var diyetisyenler = await context.Diyetisyenler.ToListAsync();
                
                if (diyetisyenler.Any())
                {
                    var diyetProgramlari = new List<DiyetProgrami>
                    {
                        new DiyetProgrami
                        {
                            Id = Guid.NewGuid(),
                            Ad = "Kilo Verme Programı",
                            Aciklama = "Sağlıklı kilo vermeyi amaçlayan program",
                            BaslangicTarihi = DateTime.Now,
                            BitisTarihi = DateTime.Now.AddMonths(3),
                            YagGram = 50,
                            ProteinGram = 120,
                            KarbonhidratGram = 150,
                            GunlukAdimHedefi = 10000,
                            OlusturanDiyetisyenId = diyetisyenler[0].Id
                        },
                        new DiyetProgrami
                        {
                            Id = Guid.NewGuid(),
                            Ad = "Kas Kazanma Programı",
                            Aciklama = "Kas kütlesi artırmaya yönelik beslenme programı",
                            BaslangicTarihi = DateTime.Now,
                            BitisTarihi = DateTime.Now.AddMonths(6),
                            YagGram = 70,
                            ProteinGram = 180,
                            KarbonhidratGram = 220,
                            GunlukAdimHedefi = 8000,
                            OlusturanDiyetisyenId = diyetisyenler[1].Id
                        }
                    };
                    
                    await context.DiyetProgramlari.AddRangeAsync(diyetProgramlari);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}