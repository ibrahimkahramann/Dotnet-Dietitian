using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using System.Security.Claims;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.HastaQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Results.HastaResults;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetProgramiQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.RandevuQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetisyenQueries;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Dotnet_Dietitian.Application.Features.CQRS.Commands.HastaCommands;
using Dotnet_Dietitian.Application.Features.CQRS.Commands.AppUserCommands;
using Dotnet_Dietitian.Application.Features.CQRS.Commands.RandevuCommands;
using Dotnet_Dietitian.Application.Features.CQRS.Commands.KullaniciAyarlariCommands;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.KullaniciAyarlariQueries;

namespace Dotnet_Dietitian.API.Controllers
{
    [Authorize(Roles = "Hasta, Admin")]
    public class PatientController : Controller
    {
        private readonly IMediator _mediator;

        public PatientController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var hastaId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // GetHastaWithDiyetProgramiQuery ile hasta verilerini ve diyet programını getir
                var hastaModel = await _mediator.Send(new GetHastaWithDiyetProgramiQuery(hastaId));
                
                // Su tüketimi - Günlük - Hedef verisi - Örnek değerler (Veritabanından gelebilir)
                ViewBag.SuTuketimi = new { Gunluk = 1.2, Hedef = 2.5 };
                
                // Adım sayısı - Günlük - Hedef verisi - Örnek değerler (Veritabanından gelebilir)
                ViewBag.AdimSayisi = new { Gunluk = 4526, Hedef = 10000 };
                
                // Kalori tüketimi - Günlük - Hedef verisi - Örnek değerler (Veritabanından gelebilir)
                ViewBag.KaloriTuketimi = new { Gunluk = 1250, Hedef = hastaModel.GunlukKaloriIhtiyaci ?? 1800 };
                
                // İlerleme takibi verileri - Örnek değerler (Veritabanından gelebilir)
                ViewBag.IlerlemeVerileri = new {
                    KiloHedefi = new { Deger = hastaModel.Kilo ?? 0, Hedef = 75, YuzdeTamamlanma = 66 },
                    EgzersizPlani = new { Deger = 3, Hedef = 5, YuzdeTamamlanma = 60 },
                    DiyetUyumu = new { YuzdeTamamlanma = 80 },
                    SonGuncelleme = DateTime.Now.AddDays(-2)
                };
                
                // Yaklaşan randevular (ilk 2 randevu)
                var yaklaşanRandevular = hastaModel.Randevular?
                    .Where(r => r.RandevuBaslangicTarihi > DateTime.Now)
                    .OrderBy(r => r.RandevuBaslangicTarihi)
                    .Take(2)
                    .ToList() ?? new List<Dotnet_Dietitian.Application.Features.CQRS.Results.HastaResults.RandevuDto>();
                    
                ViewBag.YaklasanRandevular = yaklaşanRandevular;
                
                // Diyet programı detayları - örnek veriler (Gerçek veritabanı verisi ile değiştirilmeli)
                if (hastaModel.DiyetProgramiId.HasValue)
                {
                    ViewBag.DiyetProgramiDetaylari = new {
                        Kahvalti = new List<string>
                        {
                            "1 dilim tam tahıllı ekmek",
                            "2 yemek kaşığı peynir",
                            "1 adet haşlanmış yumurta",
                            "1 bardak süt"
                        },
                        OgleYemegi = new List<string>
                        {
                            "1 kase sebze çorbası",
                            "120g ızgara tavuk",
                            "1 kase salata",
                            "1 dilim tam tahıllı ekmek"
                        }
                    };
                }
                
                return View(hastaModel);
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama yapılabilir
                ViewBag.ErrorMessage = "Hasta bilgileri getirilirken bir hata oluştu: " + ex.Message;
                return View(new GetHastaByIdQueryResult());
            }
        }
        
        public async Task<IActionResult> DietProgram()
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var hastaId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // GetHastaWithDiyetProgramiQuery ile hasta verilerini ve diyet programını getir
                var hastaModel = await _mediator.Send(new GetHastaWithDiyetProgramiQuery(hastaId));
                
                // Eğer hastanın bir diyet programı varsa, diyet programı detaylarını getir
                if (hastaModel.DiyetProgramiId.HasValue)
                {
                    // Diyet programının tüm detaylarını getir
                    var diyetProgrami = await _mediator.Send(new GetDiyetProgramiByIdQuery(hastaModel.DiyetProgramiId.Value));
                    
                    // ViewBag üzerinden diyet programı detaylarını gönder
                    ViewBag.DiyetProgrami = diyetProgrami;
                    
                    // Örnek öğünler - Bu veriler gerçek verilerle değiştirilmelidir
                    ViewBag.Ogunler = new
                    {
                        Kahvalti = new List<string>
                        {
                            "1 dilim tam tahıllı ekmek",
                            "2 yemek kaşığı lor peyniri",
                            "1 adet haşlanmış yumurta",
                            "5-6 adet zeytin",
                            "Domates, salatalık",
                            "1 bardak süt veya ayran"
                        },
                        AraOgun1 = new List<string>
                        {
                            "1 adet orta boy meyve",
                            "5-6 adet badem veya fındık"
                        },
                        OgleYemegi = new List<string>
                        {
                            "1 kase sebze çorbası",
                            "120g ızgara tavuk göğsü",
                            "1 kase salata (zeytinyağı ve limon ile)",
                            "1/2 su bardağı bulgur pilavı"
                        },
                        AraOgun2 = new List<string>
                        {
                            "1 adet yoğurt",
                            "1 tatlı kaşığı bal"
                        },
                        AksamYemegi = new List<string>
                        {
                            "150g ızgara balık",
                            "1 porsiyon haşlanmış sebze",
                            "1 dilim tam tahıllı ekmek"
                        }
                    };
                    
                    // Haftalık program özeti
                    ViewBag.HaftalikProgramOzeti = new
                    {
                        HaftalikHedef = "1-2 kg kilo kaybı",
                        SuTuketimHedefi = "Günlük 2.5 litre",
                        EgzersizHedefi = "Haftada 3 gün, 30 dakika yürüyüş",
                        KaloriHedefi = diyetProgrami.KarbonhidratGram.HasValue && diyetProgrami.ProteinGram.HasValue && diyetProgrami.YagGram.HasValue
                            ? (diyetProgrami.KarbonhidratGram.Value * 4 + diyetProgrami.ProteinGram.Value * 4 + diyetProgrami.YagGram.Value * 9).ToString("0") + " kcal"
                            : hastaModel.GunlukKaloriIhtiyaci.HasValue ? hastaModel.GunlukKaloriIhtiyaci.Value.ToString() + " kcal" : "1800 kcal"
                    };
                    
                    // Diyetisyen notları
                    ViewBag.DiyetisyenNotlari = new List<object>
                    {
                        new
                        {
                            Tarih = DateTime.Now.AddDays(-5),
                            Not = "Programın ilk haftasında su tüketimine dikkat edin. Egzersiz yoğunluğunu kademeli olarak artırın."
                        },
                        new
                        {
                            Tarih = DateTime.Now.AddDays(-2),
                            Not = "Şeker ve tuz alımınızı sınırlandırmaya özen gösterin. Beslenmede düzenli olarak yeşil yapraklı sebzelere yer verin."
                        }
                    };
                }
                
                return View(hastaModel);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Diyet programı bilgileri getirilirken bir hata oluştu: " + ex.Message;
                return View(new GetHastaByIdQueryResult());
            }
        }
        
        public async Task<IActionResult> Appointments(bool showPast = false)
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var hastaId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Hasta verilerini getir
                var hastaModel = await _mediator.Send(new GetHastaByIdQuery(hastaId));
                
                // Hastaya atanmış diyetisyen varsa sadece onu göster
                if (hastaModel.DiyetisyenId.HasValue)
                {
                    var diyetisyen = await _mediator.Send(new GetDiyetisyenByIdQuery(hastaModel.DiyetisyenId.Value));
                    if (diyetisyen != null)
                    {
                        ViewBag.Diyetisyenler = new List<object> { diyetisyen };
                        ViewBag.AtananDiyetisyenId = hastaModel.DiyetisyenId.Value;
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Atanan diyetisyen bilgilerine erişilemedi.";
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Henüz size atanmış bir diyetisyen bulunmuyor. Randevu oluşturabilmek için önce bir diyetisyen atanmalıdır.";
                }
                
                ViewData["ShowPast"] = showPast;
                return View(hastaModel);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Randevu bilgileri getirilirken bir hata oluştu: " + ex.Message;
                return View();
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> RequestAppointment(IFormCollection formData)
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var hastaId))
                {
                    return RedirectToAction("Login", "Account");
                }
                
                // Form verilerini al
                if (!Guid.TryParse(formData["DiyetisyenId"], out var diyetisyenId))
                {
                    TempData["ErrorMessage"] = "Geçersiz diyetisyen seçimi.";
                    return RedirectToAction("Appointments");
                }
                
                string randevuTarihi = formData["RandevuTarihi"];
                string randevuSaati = formData["RandevuSaati"];
                string randevuBitisSaati = formData["RandevuBitisSaati"];
                
                if (string.IsNullOrEmpty(randevuTarihi) || string.IsNullOrEmpty(randevuSaati) || string.IsNullOrEmpty(randevuBitisSaati))
                {
                    TempData["ErrorMessage"] = "Lütfen randevu tarih ve saatlerini doldurun.";
                    return RedirectToAction("Appointments");
                }
                
                // Tarih ve saatleri birleştirerek DateTime'a çevir
                DateTime baslangicTarihi, bitisTarihi;
                
                try
                {
                    baslangicTarihi = DateTime.Parse($"{randevuTarihi} {randevuSaati}");
                    bitisTarihi = DateTime.Parse($"{randevuTarihi} {randevuBitisSaati}");
                }
                catch
                {
                    TempData["ErrorMessage"] = "Geçersiz tarih veya saat formatı.";
                    return RedirectToAction("Appointments");
                }
                
                // Başlangıç tarihi şu andan sonra olmalı
                if (baslangicTarihi <= DateTime.Now)
                {
                    TempData["ErrorMessage"] = "Randevu tarihi şu andan sonra olmalıdır.";
                    return RedirectToAction("Appointments");
                }
                
                // Bitiş tarihi başlangıç tarihinden sonra olmalı
                if (bitisTarihi <= baslangicTarihi)
                {
                    TempData["ErrorMessage"] = "Bitiş saati başlangıç saatinden sonra olmalıdır.";
                    return RedirectToAction("Appointments");
                }
                
                // Randevu komutunu oluştur
                var command = new CreateRandevuCommand
                {
                    HastaId = hastaId,
                    DiyetisyenId = diyetisyenId,
                    RandevuBaslangicTarihi = baslangicTarihi,
                    RandevuBitisTarihi = bitisTarihi,
                    RandevuTuru = formData["RandevuTuru"],
                    Notlar = formData["Notlar"],
                    Durum = "Bekliyor",
                    HastaOnayi = true,
                    DiyetisyenOnayi = false,
                    YaratilmaTarihi = DateTime.Now
                };
                
                // Randevu oluştur
                await _mediator.Send(command);
                
                TempData["SuccessMessage"] = "Randevu talebiniz başarıyla alınmıştır. Diyetisyen onayı sonrası aktif olacaktır.";
                return RedirectToAction("Appointments");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Randevu oluşturulurken bir hata oluştu: " + ex.Message;
                return RedirectToAction("Appointments");
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> CancelAppointment(Guid appointmentId)
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var hastaId))
                {
                    return RedirectToAction("Login", "Account");
                }
                
                // Randevu bilgilerini getir
                var randevu = await _mediator.Send(new GetRandevuByIdQuery(appointmentId));
                
                // Randevu kullanıcıya ait mi kontrol et
                if (randevu == null || randevu.HastaId != hastaId)
                {
                    TempData["ErrorMessage"] = "Randevu bulunamadı veya iptal etme yetkiniz yok.";
                    return RedirectToAction("Appointments");
                }
                
                // Geçmiş randevular iptal edilemez
                if (randevu.RandevuBaslangicTarihi < DateTime.Now)
                {
                    TempData["ErrorMessage"] = "Geçmiş randevular iptal edilemez.";
                    return RedirectToAction("Appointments");
                }
                
                // Randevuyu iptal et
                await _mediator.Send(new RemoveRandevuCommand(appointmentId));
                
                TempData["SuccessMessage"] = "Randevu başarıyla iptal edildi.";
                return RedirectToAction("Appointments");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Randevu iptal edilirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("Appointments");
            }
        }
          public async Task<IActionResult> Messages()
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var hastaId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Hasta verilerini diyetisyen bilgileriyle birlikte getir
                var hastaModel = await _mediator.Send(new GetHastaWithDiyetProgramiQuery(hastaId));
                
                // Mesajları getirmek için sorgu yapılabilir
                // var mesajlar = await _mediator.Send(new GetMesajlarByHastaIdQuery(hastaId));
                
                return View(hastaModel);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Mesaj bilgileri getirilirken bir hata oluştu: " + ex.Message;
                return View();
            }
        }
        
        public async Task<IActionResult> Profile()
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var hastaId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Hasta verilerini getir
                var hastaModel = await _mediator.Send(new GetHastaByIdQuery(hastaId));
                return View(hastaModel);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Profil bilgileri getirilirken bir hata oluştu: " + ex.Message;
                return View();
            }
        }
        
        public async Task<IActionResult> Settings(string tab = "general")
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var hastaId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Hasta verilerini getir
                var hastaModel = await _mediator.Send(new GetHastaByIdQuery(hastaId));
                
                // Kullanıcı ayarlarını getir
                var ayarlar = await _mediator.Send(new GetKullaniciAyarlariByKullaniciIdQuery(hastaId, "Hasta"));
                
                // Ayarları ViewBag'e ekle
                ViewBag.Ayarlar = ayarlar;
                
                // Aktif tab bilgisini ViewBag'e ekle
                ViewBag.ActiveTab = tab;
                
                return View(hastaModel);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Ayar bilgileri getirilirken bir hata oluştu: " + ex.Message;
                return View();
            }
        }
        
        public async Task<IActionResult> ProgressTracking()
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var hastaId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Hasta verilerini getir
                var hastaModel = await _mediator.Send(new GetHastaByIdQuery(hastaId));
                
                // İlerleme takibi verilerini getirmek için ek sorgular yapılabilir
                // var ilerlemeVerileri = await _mediator.Send(new GetIlerlemeByHastaIdQuery(hastaId));
                
                return View(hastaModel);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "İlerleme bilgileri getirilirken bir hata oluştu: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdateHastaProfileCommand command)
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var hastaId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Güvenlik kontrolü: Sadece kendi profilini güncelleyebilir
                if (command.Id != hastaId)
                {
                    return Unauthorized();
                }

                // Profil güncelleme işlemini gerçekleştir
                await _mediator.Send(command);

                // Başarılı mesajı ile profil sayfasına yönlendir
                TempData["SuccessMessage"] = "Profil bilgileriniz başarıyla güncellendi.";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama yapılabilir
                ViewBag.ErrorMessage = "Profil güncellenirken bir hata oluştu: " + ex.Message;
                
                // Hata durumunda profil verilerini tekrar getir
                var model = await _mediator.Send(new GetHastaByIdQuery(command.Id));
                return View("Profile", model);
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> UpdatePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            try
            {
                // Şifrelerin eşleştiğini kontrol et
                if (newPassword != confirmPassword)
                {
                    TempData["ErrorMessage"] = "Yeni şifreler eşleşmiyor.";
                    return RedirectToAction("Settings", new { tab = "privacy" });
                }

                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var hastaId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // CQRS ile şifre değiştirme işlemi
                var updatePasswordCommand = new UpdatePasswordCommand
                {
                    UserId = hastaId,
                    CurrentPassword = currentPassword,
                    NewPassword = newPassword
                };

                try
                {
                    var result = await _mediator.Send(updatePasswordCommand);
                    
                    // Başarılı mesajı ile ayarlar sayfasına yönlendir
                    TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirildi.";
                    return RedirectToAction("Settings", new { tab = "privacy" });
                }
                catch (Exception ex)
                {
                    // Handler'dan gelen hata mesajlarını TempData'ya ekle
                    TempData["ErrorMessage"] = ex.Message;
                    return RedirectToAction("Settings", new { tab = "privacy" });
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama yapılabilir
                TempData["ErrorMessage"] = "Şifre değiştirilirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("Settings", new { tab = "privacy" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSettings(string settingType, IFormCollection formData)
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var hastaId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // UpdateKullaniciAyarlariCommand oluştur
                var command = new UpdateKullaniciAyarlariCommand
                {
                    KullaniciId = hastaId,
                    KullaniciTipi = "Hasta",
                    AyarTipi = settingType
                };

                // Form verilerini komuta ekle
                switch (settingType)
                {
                    case "general":
                        command.Dil = formData["language"];
                        command.ZamanDilimi = formData["timezone"];
                        command.TarihFormati = formData["dateFormat"];
                        command.OlcuBirimi = formData["weightUnit"];
                        break;
                        
                    case "notifications":
                        command.EmailRandevuBildirimleri = formData["emailNotifAppointments"] == "true";
                        command.EmailMesajBildirimleri = formData["emailNotifMessages"] == "true";
                        command.EmailDiyetGuncellemeBildirimleri = formData["emailNotifDietUpdates"] == "true";
                        command.EmailPazarlamaBildirimleri = formData["emailNotifMarketing"] == "true";
                        
                        command.UygulamaRandevuBildirimleri = formData["appNotifAppointments"] == "true";
                        command.UygulamaMesajBildirimleri = formData["appNotifMessages"] == "true";
                        command.UygulamaDiyetGuncellemeBildirimleri = formData["appNotifDietUpdates"] == "true";
                        command.UygulamaGunlukHatirlatmalar = formData["appNotifDailyReminders"] == "true";
                        break;
                        
                    case "privacy":
                        command.YeniGirisUyarilari = formData["loginAlerts"] == "true";
                        command.OturumZamanAsimi = formData["sessionTimeout"] == "true";
                        command.SaglikVerisiPaylasimiIzni = formData["shareHealthData"] == "true";
                        command.AktiviteVerisiPaylasimiIzni = formData["shareActivityData"] == "true";
                        command.AnonimKullanimVerisiPaylasimiIzni = formData["shareUsageData"] == "true";
                        break;
                        
                    case "appearance":
                        command.Tema = formData["theme"];
                        command.PanelDuzeni = formData["dashboardLayout"];
                        command.IlerlemeGrafigiGoster = formData["showProgress"] == "true";
                        command.SuTakibiGoster = formData["showWaterTracker"] == "true";
                        command.KaloriTakibiGoster = formData["showCalories"] == "true";
                        break;
                        
                    default:
                        TempData["ErrorMessage"] = "Geçersiz ayar tipi.";
                        return RedirectToAction("Settings");
                }

                // Ayarları güncelle
                await _mediator.Send(command);

                // Başarılı mesajı ile ayarlar sayfasına yönlendir
                TempData["SuccessMessage"] = "Ayarlarınız başarıyla güncellendi.";
                return RedirectToAction("Settings", new { tab = settingType });
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama yapılabilir
                TempData["ErrorMessage"] = "Ayarlar güncellenirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("Settings", new { tab = settingType });
            }
        }
    }
}