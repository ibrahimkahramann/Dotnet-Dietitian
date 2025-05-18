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
            try
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Önce roller oluşturulur ve veritabanına kaydedilir
                if (!context.AppRoles.Any())
                {
                    var roles = new List<AppRole>
                    {
                        new AppRole { Id = Guid.NewGuid(), AppRoleName = "Admin" },
                        new AppRole { Id = Guid.NewGuid(), AppRoleName = "Diyetisyen" },
                        new AppRole { Id = Guid.NewGuid(), AppRoleName = "Hasta" }
                    };
                    
                    await context.AppRoles.AddRangeAsync(roles);
                    await context.SaveChangesAsync();
                }

                // Roller veritabanından çekilir
                var allRoles = await context.AppRoles.ToListAsync();
                var adminRoleId = allRoles.First(r => r.AppRoleName == "Admin").Id;
                var diyetisyenRoleId = allRoles.First(r => r.AppRoleName == "Diyetisyen").Id;
                var hastaRoleId = allRoles.First(r => r.AppRoleName == "Hasta").Id;

                // Diyetisyenler
                List<Diyetisyen> diyetisyenler = new List<Diyetisyen>();
                if (!context.Diyetisyenler.Any())
                {
                    diyetisyenler = new List<Diyetisyen>
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
                        },
                        new Diyetisyen
                        {
                            Id = Guid.NewGuid(),
                            TcKimlikNumarasi = "34567890123",
                            Ad = "Mehmet",
                            Soyad = "Kaya",
                            Uzmanlik = "Klinik Beslenme",
                            Email = "mehmet.kaya@example.com",
                            Telefon = "5554567890",
                            MezuniyetOkulu = "Hacettepe Üniversitesi",
                            DeneyimYili = 12,
                            Puan = 4.9f,
                            ToplamYorumSayisi = 87,
                            Hakkinda = "Kronik hastalıklarda beslenme tedavisi konusunda uzmanım.",
                            Sehir = "İzmir"
                        },
                        new Diyetisyen
                        {
                            Id = Guid.NewGuid(),
                            TcKimlikNumarasi = "45678901234",
                            Ad = "Zeynep",
                            Soyad = "Öztürk",
                            Uzmanlik = "Obezite ve Metabolik Hastalıklar",
                            Email = "zeynep.ozturk@example.com",
                            Telefon = "5551122334",
                            MezuniyetOkulu = "Ege Üniversitesi",
                            DeneyimYili = 7,
                            Puan = 4.6f,
                            ToplamYorumSayisi = 42,
                            Hakkinda = "Obezite tedavisi ve metabolik sendrom yönetimi konularında çalışıyorum.",
                            Sehir = "İzmir"
                        },
                        new Diyetisyen
                        {
                            Id = Guid.NewGuid(),
                            TcKimlikNumarasi = "56789012345",
                            Ad = "Mustafa",
                            Soyad = "Şahin",
                            Uzmanlik = "Fonksiyonel Tıp ve Beslenme",
                            Email = "mustafa.sahin@example.com",
                            Telefon = "5553344556",
                            MezuniyetOkulu = "Marmara Üniversitesi",
                            DeneyimYili = 9,
                            Puan = 4.7f,
                            ToplamYorumSayisi = 56,
                            Hakkinda = "Fonksiyonel tıp çerçevesinde kişiselleştirilmiş beslenme programları oluşturuyorum.",
                            Sehir = "İstanbul"
                        }
                    };
                    
                    await context.Diyetisyenler.AddRangeAsync(diyetisyenler);
                    await context.SaveChangesAsync();
                }
                else
                {
                    // Eğer diyetisyenler zaten varsa onları alıyoruz
                    diyetisyenler = await context.Diyetisyenler.ToListAsync();
                }
                
                // AppUser ekleme
                if (!context.AppUsers.Any() && diyetisyenler.Any())
                {
                    var users = new List<AppUser>
                    {
                        new AppUser { Id = Guid.NewGuid(), Username = "admin", Password = "admin123", AppRoleId = adminRoleId },
                        new AppUser { Id = Guid.NewGuid(), Username = "ahmetyilmaz", Password = "123456", AppRoleId = diyetisyenRoleId },
                        new AppUser { Id = Guid.NewGuid(), Username = "aysedemir", Password = "123456", AppRoleId = diyetisyenRoleId },
                    };
                    
                    await context.AppUsers.AddRangeAsync(users);
                    await context.SaveChangesAsync();
                }
                
                // Diyet Programları
                List<DiyetProgrami> diyetProgramlari = new List<DiyetProgrami>();
                if (!context.DiyetProgramlari.Any() && diyetisyenler.Any())
                {
                    diyetProgramlari = new List<DiyetProgrami>
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
                        },
                        // Yeni diyet programları
                        new DiyetProgrami
                        {
                            Id = Guid.NewGuid(),
                            Ad = "Akdeniz Diyeti",
                            Aciklama = "Kalp sağlığını destekleyen ve genel sağlığı iyileştiren Akdeniz tipi beslenme programı",
                            BaslangicTarihi = DateTime.Now.AddDays(-15),
                            BitisTarihi = DateTime.Now.AddMonths(4),
                            YagGram = 65,
                            ProteinGram = 90,
                            KarbonhidratGram = 180,
                            GunlukAdimHedefi = 7500,
                            OlusturanDiyetisyenId = diyetisyenler[2].Id
                        },
                        new DiyetProgrami
                        {
                            Id = Guid.NewGuid(),
                            Ad = "Diyabet Yönetim Programı",
                            Aciklama = "Kan şekeri kontrolüne yardımcı olan düşük glisemik indeksli beslenme programı",
                            BaslangicTarihi = DateTime.Now.AddDays(-30),
                            BitisTarihi = DateTime.Now.AddMonths(6),
                            YagGram = 60,
                            ProteinGram = 100,
                            KarbonhidratGram = 130,
                            GunlukAdimHedefi = 9000,
                            OlusturanDiyetisyenId = diyetisyenler[2].Id
                        }
                    };

                    // Eğer yeterli diyetisyen varsa 5. programı ekle
                    if (diyetisyenler.Count >= 5)
                    {
                        diyetProgramlari.Add(new DiyetProgrami
                        {
                            Id = Guid.NewGuid(),
                            Ad = "Vejetaryen Beslenme Planı",
                            Aciklama = "Bitkisel ağırlıklı, protein ihtiyacını karşılayan beslenme programı",
                            BaslangicTarihi = DateTime.Now.AddDays(-5),
                            BitisTarihi = DateTime.Now.AddMonths(3),
                            YagGram = 55,
                            ProteinGram = 80,
                            KarbonhidratGram = 200,
                            GunlukAdimHedefi = 6000,
                            OlusturanDiyetisyenId = diyetisyenler[4].Id
                        });
                    }
                    
                    await context.DiyetProgramlari.AddRangeAsync(diyetProgramlari);
                    await context.SaveChangesAsync();
                }
                else
                {
                    // Eğer programlar zaten varsa onları alıyoruz
                    diyetProgramlari = await context.DiyetProgramlari.ToListAsync();
                }
                
                // Hastalar
                List<Hasta> hastalar = new List<Hasta>();
                if (!context.Hastalar.Any() && diyetisyenler.Any() && diyetProgramlari.Any())
                {
                    // Diyetisyen ve program indekslerini güvenli hale getiriyoruz
                    var diyet0Id = diyetProgramlari.Count > 0 ? diyetProgramlari[0].Id : Guid.Empty;
                    var diyet1Id = diyetProgramlari.Count > 1 ? diyetProgramlari[1].Id : diyet0Id;
                    var diyet3Id = diyetProgramlari.Count > 3 ? diyetProgramlari[3].Id : diyet0Id;
                    var diyet4Id = diyetProgramlari.Count > 4 ? diyetProgramlari[4].Id : diyet0Id;
                    
                    var diyetisyen0Id = diyetisyenler[0].Id;
                    var diyetisyen1Id = diyetisyenler.Count > 1 ? diyetisyenler[1].Id : diyetisyen0Id;
                    var diyetisyen2Id = diyetisyenler.Count > 2 ? diyetisyenler[2].Id : diyetisyen0Id;
                    var diyetisyen4Id = diyetisyenler.Count > 4 ? diyetisyenler[4].Id : diyetisyen0Id;
                    
                    hastalar = new List<Hasta>
                    {
                        new Hasta
                        {
                            Id = Guid.NewGuid(),
                            TcKimlikNumarasi = "67890123456",
                            Ad = "Ali",
                            Soyad = "Yılmaz",
                            Yas = 35,
                            Boy = 180,
                            Kilo = 85,
                            Email = "ali.yilmaz@example.com",
                            Telefon = "5555678901",
                            DiyetisyenId = diyetisyen0Id,
                            DiyetProgramiId = diyet0Id,
                            GunlukKaloriIhtiyaci = 2200
                        },
                        new Hasta
                        {
                            Id = Guid.NewGuid(),
                            TcKimlikNumarasi = "78901234567",
                            Ad = "Ayşe",
                            Soyad = "Kaya",
                            Yas = 28,
                            Boy = 165,
                            Kilo = 62,
                            Email = "ayse.kaya@example.com",
                            Telefon = "5556789012",
                            DiyetisyenId = diyetisyen1Id,
                            DiyetProgramiId = diyet1Id,
                            GunlukKaloriIhtiyaci = 1800
                        },
                        new Hasta
                        {
                            Id = Guid.NewGuid(),
                            TcKimlikNumarasi = "89012345678",
                            Ad = "Mehmet",
                            Soyad = "Demir",
                            Yas = 42,
                            Boy = 175,
                            Kilo = 90,
                            Email = "mehmet.demir@example.com",
                            Telefon = "5557890123",
                            DiyetisyenId = diyetisyen2Id,
                            DiyetProgramiId = diyet3Id,
                            GunlukKaloriIhtiyaci = 2000
                        },
                        new Hasta
                        {
                            Id = Guid.NewGuid(),
                            TcKimlikNumarasi = "90123456789",
                            Ad = "Zeynep",
                            Soyad = "Şahin",
                            Yas = 25,
                            Boy = 168,
                            Kilo = 55,
                            Email = "zeynep.sahin@example.com",
                            Telefon = "5558901234",
                            DiyetisyenId = diyetisyen1Id,
                            DiyetProgramiId = diyet0Id,
                            GunlukKaloriIhtiyaci = 1600
                        }
                    };
                    
                    // Diğer iki hasta değerlerini güvenle ekliyoruz
                    if (diyetisyenler.Count >= 5 && diyetProgramlari.Count >= 5)
                    {
                        hastalar.Add(new Hasta
                        {
                            Id = Guid.NewGuid(),
                            TcKimlikNumarasi = "01234567890",
                            Ad = "Burak",
                            Soyad = "Aksoy",
                            Yas = 32,
                            Boy = 185,
                            Kilo = 92,
                            Email = "burak.aksoy@example.com",
                            Telefon = "5559012345",
                            DiyetisyenId = diyetisyen0Id,
                            DiyetProgramiId = diyet4Id,
                            GunlukKaloriIhtiyaci = 2500
                        });
                        
                        hastalar.Add(new Hasta
                        {
                            Id = Guid.NewGuid(),
                            TcKimlikNumarasi = "12389456780",
                            Ad = "Selin",
                            Soyad = "Özdemir",
                            Yas = 29,
                            Boy = 170,
                            Kilo = 65,
                            Email = "selin.ozdemir@example.com",
                            Telefon = "5550123456",
                            DiyetisyenId = diyetisyen4Id,
                            DiyetProgramiId = diyet4Id,
                            GunlukKaloriIhtiyaci = 1900
                        });
                    }
                    
                    await context.Hastalar.AddRangeAsync(hastalar);
                    await context.SaveChangesAsync();

                    // Hasta kullanıcıları ekliyoruz
                    var hastaUsers = new List<AppUser>();
                    
                    for (int i = 0; i < Math.Min(hastalar.Count, 3); i++)
                    {
                        hastaUsers.Add(new AppUser
                        {
                            Id = Guid.NewGuid(),
                            Username = hastalar[i].Ad.ToLower() + hastalar[i].Soyad.ToLower(),
                            Password = "123456",
                            AppRoleId = hastaRoleId // Hasta rolü
                        });
                    }
                    
                    await context.AppUsers.AddRangeAsync(hastaUsers);
                    await context.SaveChangesAsync();
                }
                else
                {
                    // Eğer hastalar zaten varsa onları alıyoruz
                    hastalar = await context.Hastalar.ToListAsync();
                }
                
                // Diyetisyen Uygunluk
                if (!context.DiyetisyenUygunluklar.Any() && diyetisyenler.Any())
                {
                    var uygunlukZamanlari = new List<DiyetisyenUygunluk>();
                    
                    // Tüm diyetisyenler için uygunluk saatleri ekle
                    for (int i = 0; i < diyetisyenler.Count; i++)
                    {
                        // Her diyetisyen için iki uygun zaman dilimi ekle
                        uygunlukZamanlari.Add(new DiyetisyenUygunluk
                        {
                            Id = Guid.NewGuid(),
                            DiyetisyenId = diyetisyenler[i].Id,
                            Gun = DateTime.Now.AddDays(i % 5 + 1), // 1-5 arası günler
                            BaslangicSaati = new TimeSpan(9, 0, 0),
                            BitisSaati = new TimeSpan(12, 0, 0),
                            TekrarTipi = "Haftalık"
                        });
                        
                        uygunlukZamanlari.Add(new DiyetisyenUygunluk
                        {
                            Id = Guid.NewGuid(),
                            DiyetisyenId = diyetisyenler[i].Id,
                            Gun = DateTime.Now.AddDays(i % 5 + 1),
                            BaslangicSaati = new TimeSpan(14, 0, 0),
                            BitisSaati = new TimeSpan(17, 0, 0),
                            TekrarTipi = "Haftalık"
                        });
                    }
                    
                    await context.DiyetisyenUygunluklar.AddRangeAsync(uygunlukZamanlari);
                    await context.SaveChangesAsync();
                }
                
                // Randevular
                if (!context.Randevular.Any() && diyetisyenler.Any() && hastalar.Any())
                {
                    var randevular = new List<Randevu>();
                    
                    // Güvenli bir şekilde randevuları ekleyelim
                    int hastaCount = hastalar.Count;
                    int diyetisyenCount = diyetisyenler.Count;
                    
                    // Her hasta için bir randevu ekliyoruz (mevcut hasta sayısı kadar)
                    for (int i = 0; i < Math.Min(hastaCount, 5); i++)
                    {
                        var diyetisyenIndex = i % diyetisyenCount; // Diyetisyen dizisi sınırları içinde kal
                        
                        randevular.Add(new Randevu
                        {
                            Id = Guid.NewGuid(),
                            HastaId = hastalar[i].Id,
                            DiyetisyenId = diyetisyenler[diyetisyenIndex].Id,
                            RandevuBaslangicTarihi = DateTime.Now.AddDays(i + 1).AddHours(10),
                            RandevuBitisTarihi = DateTime.Now.AddDays(i + 1).AddHours(11),
                            RandevuTuru = i == 0 ? "İlk Görüşme" : (i % 3 == 0 ? "Kontrol" : "Diyet Planı Güncelleme"),
                            Durum = i % 4 == 0 ? "Bekliyor" : "Onaylandı",
                            DiyetisyenOnayi = i % 4 != 0,
                            HastaOnayi = i % 4 != 0,
                            Notlar = $"Randevu notları {i + 1}",
                            YaratilmaTarihi = DateTime.Now.AddDays(-i)
                        });
                    }
                    
                    await context.Randevular.AddRangeAsync(randevular);
                    await context.SaveChangesAsync();
                }
                
                // Ödeme Bilgileri
                if (!context.OdemeBilgileri.Any() && hastalar.Any())
                {
                    var odemeler = new List<OdemeBilgisi>();
                    
                    // Her hasta için en az bir ödeme ekle
                    for (int i = 0; i < hastalar.Count; i++)
                    {
                        odemeler.Add(new OdemeBilgisi
                        {
                            Id = Guid.NewGuid(),
                            HastaId = hastalar[i].Id,
                            Tutar = 500 + (i * 100), // Farklı tutarlar
                            Tarih = DateTime.Now.AddDays(-i * 2 - 1),
                            OdemeTuru = i % 3 == 0 ? "Kredi Kartı" : (i % 3 == 1 ? "Havale" : "Nakit"),
                            Aciklama = $"Diyet programı ödemesi - Hasta {i + 1}",
                            IslemReferansNo = $"REF{10000 + i}"
                        });
                    }
                    
                    // Bazı hastalara ek ödemeler ekle
                    if (hastalar.Count >= 2)
                    {
                        odemeler.Add(new OdemeBilgisi
                        {
                            Id = Guid.NewGuid(),
                            HastaId = hastalar[0].Id,
                            Tutar = 200,
                            Tarih = DateTime.Now.AddDays(-1),
                            OdemeTuru = "Kredi Kartı",
                            Aciklama = "Kontrol seansı",
                            IslemReferansNo = "REF78901"
                        });
                    }
                    
                    await context.OdemeBilgileri.AddRangeAsync(odemeler);
                    await context.SaveChangesAsync();
                }
                
                // Mesaj seed data örneği:
                if (!context.Mesajlar.Any() && hastalar.Any() && diyetisyenler.Any())
                {
                    var mesajlar = new List<Mesaj>();
                    
                    // Güvenli bir şekilde mesajları ekleyelim - değişkenleri burada tanımla
                    int hastaCount = hastalar.Count;
                    int diyetisyenCount = diyetisyenler.Count;
                    
                    // Her hasta ve diyetisyen arasında örnek mesajlaşma
                    for (int i = 0; i < Math.Min(hastaCount, 5); i++)
                    {
                        var diyetisyenIndex = i % diyetisyenCount;
                        
                        // Hastadan diyetisyene mesaj
                        mesajlar.Add(new Mesaj
                        {
                            Id = Guid.NewGuid(),
                            GonderenId = hastalar[i].Id,
                            GonderenTipi = "Hasta",
                            AliciId = diyetisyenler[diyetisyenIndex].Id,
                            AliciTipi = "Diyetisyen",
                            Icerik = $"Merhaba, ben {hastalar[i].Ad}. Bir sorum var.",
                            GonderimZamani = DateTime.Now.AddHours(-2),
                            Okundu = true,
                            OkunmaZamani = DateTime.Now.AddHours(-1)
                        });
                        
                        // Diyetisyenden hastaya yanıt
                        mesajlar.Add(new Mesaj
                        {
                            Id = Guid.NewGuid(),
                            GonderenId = diyetisyenler[diyetisyenIndex].Id,
                            GonderenTipi = "Diyetisyen",
                            AliciId = hastalar[i].Id,
                            AliciTipi = "Hasta",
                            Icerik = $"Merhaba {hastalar[i].Ad}, nasıl yardımcı olabilirim?",
                            GonderimZamani = DateTime.Now.AddHours(-1)
                        });
                    }
                    
                    await context.Mesajlar.AddRangeAsync(mesajlar);
                    await context.SaveChangesAsync();
                }
                
                Console.WriteLine("Seed verileri başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                // Hata durumunu logla
                Console.WriteLine($"Seed işlemi sırasında hata oluştu: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"İç hata: {ex.InnerException.Message}");
                }
                // Geliştirme ortamında hatayı fırlat
                throw;
            }
        }
    }
}