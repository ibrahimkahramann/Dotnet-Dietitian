using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using System.Security.Claims;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetisyenQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Results.DiyetisyenResults;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.HastaQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.RandevuQueries;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetisyenCommands;
using Dotnet_Dietitian.Application.Features.CQRS.Commands.AppUserCommands;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetProgramiQueries;
using System.IO;
using Dotnet_Dietitian.Application.Features.CQRS.Commands.RandevuCommands;
using Dotnet_Dietitian.Application.Features.CQRS.Commands.HastaCommands;
using Microsoft.Extensions.Logging;
using Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetProgramiCommands;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.KullaniciAyarlariQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Commands.KullaniciAyarlariCommands;
using Dotnet_Dietitian.Application.Features.CQRS.Commands.PaymentRequestCommands;

namespace Dotnet_Dietitian.API.Controllers
{
    [Authorize(Roles = "Diyetisyen, Admin")]
    public class DietitianController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DietitianController> _logger;

        public DietitianController(IMediator mediator, ILogger<DietitianController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Giriş yapmış kullanıcının ID'sini al
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var diyetisyenId))
            {
                try
                {
                    // Diyetisyen verilerini getir ve ViewData'ya ekle
                    var diyetisyenModel = await _mediator.Send(new GetDiyetisyenByIdQuery(diyetisyenId));
                    ViewData["DiyetisyenModel"] = diyetisyenModel;
                }
                catch (Exception)
                {
                    // Hata durumunda sessizce devam et
                }
            }

            await base.OnActionExecutionAsync(context, next);
        }

        // Yetkilendirme durumunu kontrol etmek için action metodu
        public IActionResult CheckAuth()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            
            return Json(new { 
                IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
                UserId = userId,
                UserRole = userRole,
                Claims = claims
            });
        }

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // GetDiyetisyenWithHastalarQuery ile diyetisyen verilerini ve hastalarını getir
                var diyetisyenModel = await _mediator.Send(new GetDiyetisyenWithHastalarQuery(diyetisyenId));
                
                // Diyetisyenin randevularını getir
                var randevular = await _mediator.Send(new GetRandevuByDiyetisyenIdQuery(diyetisyenId));
                ViewBag.Randevular = randevular;
                
                // Bugünkü randevuların sayısını hesapla
                var bugunRandevular = randevular.Where(r => r.RandevuBaslangicTarihi.Date == DateTime.Today).ToList();
                var dunRandevular = randevular.Where(r => r.RandevuBaslangicTarihi.Date == DateTime.Today.AddDays(-1)).ToList();
                
                // Randevulardaki artış oranını hesapla (dünden bugüne)
                var randevuArtisOrani = dunRandevular.Count > 0 
                    ? bugunRandevular.Count - dunRandevular.Count 
                    : bugunRandevular.Count;
                ViewBag.RandevulardakiArtisOrani = randevuArtisOrani;
                
                // Diyet programlarını getir
                var diyetProgramlari = await _mediator.Send(new GetDiyetProgramiQuery());
                var diyetisyenProgramlari = diyetProgramlari
                    .Where(d => d.OlusturanDiyetisyenId == diyetisyenId)
                    .ToList();
                
                // Aktif diyet planlarını sayısı - Diyetisyenin oluşturduğu ve hastalara atanmış programlar
                var hastalar = await _mediator.Send(new GetHastasByDiyetisyenIdQuery(diyetisyenId));
                var atanmisPlanIds = hastalar
                    .Where(h => h.DiyetProgramiId.HasValue)
                    .Select(h => h.DiyetProgramiId.Value)
                    .Distinct()
                    .ToList();
                    
                var aktifDiyetPlanlariSayisi = atanmisPlanIds.Count;
                ViewBag.AktifDiyetPlanlariSayisi = aktifDiyetPlanlariSayisi;
                
                // Bir önceki haftaya göre aktif diyet planlarındaki artış oranını hesapla
                // Gerçek uygulamada geçmiş hafta verisi veritabanından çekilebilir
                // Şimdilik sabit değer kullanalım
                ViewBag.DiyetPlanlarindakiArtisOrani = 3;
                
                // Hastalardaki artış oranını hesapla (Bu ay vs önceki ay)
                // Gerçek uygulamada veritabanından çekilebilir
                // Şimdilik sabit değer kullanalım
                ViewBag.HastalardakiArtisOrani = 5.7;
                
                // Okunmamış mesaj sayısı - Gerçek uygulamada veritabanından çekilebilir
                // Şimdilik sabit değer kullanalım
                ViewBag.OkunmamisMesajSayisi = 3;
                ViewBag.MesajlardakiArtisOrani = 2;
                
                // Bildirimler - Gerçek uygulamada veritabanından çekilebilir
                // Şimdilik örnek veriler oluşturalım
                var bildirimler = new List<dynamic>();
                
                // Hastalar listenin içinde olmayabilir, bu durumu kontrol edelim
                if (diyetisyenModel.Hastalar != null && diyetisyenModel.Hastalar.Any())
                {
                    bildirimler.Add(new {
                        Baslik = "Yeni randevu talebi",
                        ZamanBilgisi = "3 saat önce",
                        Icerik = $"{diyetisyenModel.Hastalar.FirstOrDefault()?.Ad} {diyetisyenModel.Hastalar.FirstOrDefault()?.Soyad} yarın için randevu talep etti.",
                        Url = "/Dietitian/Appointments"
                    });
                    
                    if (diyetisyenModel.Hastalar.Count > 1)
                    {
                        bildirimler.Add(new {
                            Baslik = "Yeni mesaj",
                            ZamanBilgisi = "5 saat önce",
                            Icerik = $"{diyetisyenModel.Hastalar.ElementAtOrDefault(1)?.Ad} {diyetisyenModel.Hastalar.ElementAtOrDefault(1)?.Soyad}: \"Diyet planım hakkında bir sorum vardı...\"",
                            Url = "/Dietitian/Messages"
                        });
                    }
                    
                    if (diyetisyenModel.Hastalar.Count > 2)
                    {
                        bildirimler.Add(new {
                            Baslik = "İptal edilen randevu",
                            ZamanBilgisi = "Dün",
                            Icerik = $"{diyetisyenModel.Hastalar.ElementAtOrDefault(2)?.Ad} {diyetisyenModel.Hastalar.ElementAtOrDefault(2)?.Soyad} bugünkü randevusunu iptal etti.",
                            Url = "/Dietitian/Appointments?showPast=true"
                        });
                    }
                }
                
                // En azından bir bildirim ekleyelim
                bildirimler.Add(new {
                    Baslik = "Yeni hasta kaydı",
                    ZamanBilgisi = "2 gün önce",
                    Icerik = "Yeni bir hasta uygulamaya kaydoldu ve sizinle çalışmak istiyor.",
                    Url = "/Dietitian/DietitianPatients"
                });
                
                ViewBag.Bildirimler = bildirimler;
                
                // Debug için hasta sayısını logla
                _logger.LogInformation($"Diyetisyen ID: {diyetisyenId}, Hasta Sayısı: {diyetisyenModel.Hastalar?.Count ?? 0}");
                
                return View(diyetisyenModel);
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama yap
                _logger.LogError(ex, "Dashboard sayfası yüklenirken hata oluştu");
                ViewBag.ErrorMessage = "Diyetisyen bilgileri getirilirken bir hata oluştu: " + ex.Message;
                return View(new GetDiyetisyenByIdQueryResult());
            }
        }
        
        public async Task<IActionResult> DietitianPatients()
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Diyetisyen verilerini getir (hastaları dahil)
                var diyetisyenModel = await _mediator.Send(new GetDiyetisyenWithHastalarQuery(diyetisyenId));
                
                // Diyetisyenin hastalarını getir
                var diyetisyenHastalari = await _mediator.Send(new GetHastasByDiyetisyenIdQuery(diyetisyenId));
                
                // Tüm hastaları getir
                var tumHastalar = await _mediator.Send(new GetHastaQuery());
                
                // Diyetisyene ait olmayan hastaları filtrele
                var diyetisyensizHastalar = tumHastalar.Where(h => h.DiyetisyenId == null).ToList();
                
                ViewBag.DiyetisyenHastalari = diyetisyenHastalari;
                ViewBag.DiyetisyensizHastalar = diyetisyensizHastalar;
                ViewBag.TumHastalar = tumHastalar;
                
                return View(diyetisyenModel);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Hasta bilgileri getirilirken bir hata oluştu: " + ex.Message;
                return View();
            }
        }
        
        public async Task<IActionResult> Appointments(bool showPast = false)
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Diyetisyen verilerini getir
                var diyetisyenModel = await _mediator.Send(new GetDiyetisyenByIdQuery(diyetisyenId));
                
                // Hastaları getirmek için sorgu yap
                var hastalar = await _mediator.Send(new GetHastasByDiyetisyenIdQuery(diyetisyenId));
                
                // Hastaları diyetisyen modeline ekle
                if (diyetisyenModel.Hastalar == null)
                {
                    diyetisyenModel.Hastalar = new List<HastaDto>();
                }
                
                // Eğer model içindeki hasta listesi boşsa, GetHastasByDiyetisyenIdQuery sonucunu kullan
                if (!diyetisyenModel.Hastalar.Any())
                {
                    diyetisyenModel.Hastalar = hastalar.Select(h => new HastaDto
                    {
                        Id = h.Id,
                        Ad = h.Ad,
                        Soyad = h.Soyad,
                        Email = h.Email
                    }).ToList();
                }
                
                // Randevuları getirmek için sorgu yap
                var randevular = await _mediator.Send(new GetRandevuByDiyetisyenIdQuery(diyetisyenId));
                
                // Geçmiş veya gelecek randevular için filtreleme
                DateTime now = DateTime.Now;
                if (showPast)
                {
                    randevular = randevular.Where(r => r.RandevuBaslangicTarihi < now).ToList();
                }
                else
                {
                    randevular = randevular.Where(r => r.RandevuBaslangicTarihi >= now).ToList();
                }
                
                ViewBag.Randevular = randevular;
                ViewData["ShowPast"] = showPast;
                return View(diyetisyenModel);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Randevu bilgileri getirilirken bir hata oluştu: " + ex.Message;
                return View();
            }
        }
        
        [HttpGet]
        [Route("Dietitian/DietPlans")]
        public async Task<IActionResult> DietPlans()
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Diyetisyen verilerini getir
                var diyetisyenModel = await _mediator.Send(new GetDiyetisyenByIdQuery(diyetisyenId));
                
                // Tüm diyet programlarını getir
                var diyetProgramlari = await _mediator.Send(new GetDiyetProgramiQuery());
                
                // Diyetisyene ait şablonları filtrele (şablonlar atanmamış programlar)
                var sablonlar = diyetProgramlari
                    .Where(d => d.OlusturanDiyetisyenId == diyetisyenId)
                    .ToList();
                
                // Atanmış programları filtrele (hasta ilişkisi olan programlar)
                // Gerçek uygulamada, bu GetHastasByDiyetisyenIdQuery ile hastalar üzerinden kontrol edilebilir
                var hastalar = await _mediator.Send(new GetHastasByDiyetisyenIdQuery(diyetisyenId));
                var atanmisPlanIds = hastalar
                    .Where(h => h.DiyetProgramiId.HasValue)
                    .Select(h => h.DiyetProgramiId.Value)
                    .Distinct()
                    .ToList();
                
                var atanmisPlanlar = diyetProgramlari
                    .Where(d => d.OlusturanDiyetisyenId == diyetisyenId && atanmisPlanIds.Contains(d.Id))
                    .ToList();
                
                ViewBag.Sablonlar = sablonlar;
                ViewBag.AtanmisPlanlar = atanmisPlanlar;
                
                // Başarı mesajı TempData'dan geliyorsa göster
                if (TempData["SuccessMessage"] != null)
                {
                    ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
                }
                
                return View(diyetisyenModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Diyet planları getirilirken hata oluştu");
                ViewBag.ErrorMessage = "Diyet planları getirilirken bir hata oluştu: " + ex.Message;
                return View();
            }
        }
          public async Task<IActionResult> Messages(Guid? patientId = null)
        {
            try
            {
                // Get the logged-in dietitian's ID
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Get dietitian details
                var diyetisyenModel = await _mediator.Send(new GetDiyetisyenByIdQuery(diyetisyenId));
                
                // Get dietitian's patients
                var hastalar = await _mediator.Send(new GetHastasByDiyetisyenIdQuery(diyetisyenId));
                ViewBag.Hastalar = hastalar;
                
                // If a specific patient is selected, set it in ViewBag
                if (patientId.HasValue)
                {
                    var selectedPatient = hastalar.FirstOrDefault(h => h.Id == patientId.Value);
                    if (selectedPatient != null)
                    {
                        ViewBag.SelectedPatient = selectedPatient;
                    }
                }
                
                return View(diyetisyenModel);
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
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Diyetisyen verilerini getir (hastalar dahil)
                var diyetisyenModel = await _mediator.Send(new GetDiyetisyenWithHastalarQuery(diyetisyenId));
                return View(diyetisyenModel);
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
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Diyetisyen verilerini getir
                var diyetisyenModel = await _mediator.Send(new GetDiyetisyenByIdQuery(diyetisyenId));
                
                // Kullanıcı ayarlarını getir
                var ayarlar = await _mediator.Send(new GetKullaniciAyarlariByKullaniciIdQuery(diyetisyenId, "Diyetisyen"));
                
                // Ayarları ViewBag'e ekle
                ViewBag.Ayarlar = ayarlar;
                
                // Aktif tab bilgisini ViewBag'e ekle
                ViewBag.ActiveTab = tab;
                
                return View(diyetisyenModel);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Ayar bilgileri getirilirken bir hata oluştu: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdateDiyetisyenCommand command)
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Güvenlik kontrolü: Sadece kendi profilini güncelleyebilir
                if (command.Id != diyetisyenId)
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
                var model = await _mediator.Send(new GetDiyetisyenByIdQuery(command.Id));
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
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // CQRS ile şifre değiştirme işlemi
                var updatePasswordCommand = new UpdatePasswordCommand
                {
                    UserId = diyetisyenId,
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
        public async Task<IActionResult> UpdateProfilePicture(Guid Id, IFormFile profileImage, string ProfilResmiUrl)
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Güvenlik kontrolü: Sadece kendi profilini güncelleyebilir
                if (Id != diyetisyenId)
                {
                    return Unauthorized();
                }

                // Diyetisyen verilerini getir
                var diyetisyen = await _mediator.Send(new GetDiyetisyenByIdQuery(diyetisyenId));
                
                string profileImageUrl = ProfilResmiUrl;
                
                // Eğer dosya yüklendiyse, kaydet ve URL'i güncelle
                if (profileImage != null && profileImage.Length > 0)
                {
                    // Dosya adını belirle
                    var fileName = $"profile_{diyetisyenId}_{DateTime.Now.Ticks}{Path.GetExtension(profileImage.FileName)}";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "profiles", fileName);
                    
                    // Klasörün var olduğundan emin ol
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    
                    // Dosyayı kaydet
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await profileImage.CopyToAsync(stream);
                    }
                    
                    // URL'i güncelle
                    profileImageUrl = $"/img/profiles/{fileName}";
                }
                
                // Profil resmini güncelle
                var updateCommand = new UpdateDiyetisyenCommand
                {
                    Id = diyetisyenId,
                    TcKimlikNumarasi = diyetisyen.TcKimlikNumarasi,
                    Ad = diyetisyen.Ad,
                    Soyad = diyetisyen.Soyad,
                    Email = diyetisyen.Email,
                    Telefon = diyetisyen.Telefon,
                    Uzmanlik = diyetisyen.Uzmanlik,
                    MezuniyetOkulu = diyetisyen.MezuniyetOkulu,
                    DeneyimYili = diyetisyen.DeneyimYili,
                    Hakkinda = diyetisyen.Hakkinda,
                    Sehir = diyetisyen.Sehir,
                    ProfilResmiUrl = profileImageUrl,
                    Unvan = diyetisyen.Unvan,
                    CalistigiKurum = diyetisyen.CalistigiKurum,
                    LisansNumarasi = diyetisyen.LisansNumarasi
                };
                
                await _mediator.Send(updateCommand);
                
                TempData["SuccessMessage"] = "Profil resminiz başarıyla güncellendi.";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Profil resmi güncellenirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("Profile");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSettings(string settingType, IFormCollection formData)
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // UpdateKullaniciAyarlariCommand oluştur
                var command = new UpdateKullaniciAyarlariCommand
                {
                    KullaniciId = diyetisyenId,
                    KullaniciTipi = "Diyetisyen",
                    AyarTipi = settingType
                };

                // Form verilerini komuta ekle
                switch (settingType)
                {
                    case "general":
                        command.Dil = formData["language"];
                        command.ZamanDilimi = formData["timezone"];
                        command.TarihFormati = formData["dateFormat"];
                        command.CalismaBaslangicSaati = formData["workStartTime"];
                        command.CalismaBitisSaati = formData["workEndTime"];
                        command.HaftaSonuCalisma = formData["weekendAvailability"] == "on";
                        break;
                        
                    case "notifications":
                        command.EmailRandevuBildirimleri = formData["emailNotifAppointments"] == "on";
                        command.EmailMesajBildirimleri = formData["emailNotifMessages"] == "on";
                        command.EmailYeniHastaBildirimleri = formData["emailNotifPatients"] == "on";
                        command.EmailPazarlamaBildirimleri = formData["emailNotifMarketing"] == "on";
                        
                        command.UygulamaRandevuBildirimleri = formData["appNotifAppointments"] == "on";
                        command.UygulamaMesajBildirimleri = formData["appNotifMessages"] == "on";
                        command.UygulamaYeniHastaBildirimleri = formData["appNotifPatients"] == "on";
                        break;
                        
                    case "privacy":
                        command.YeniGirisUyarilari = formData["loginAlerts"] == "on";
                        command.OturumZamanAsimi = formData["sessionTimeout"] == "on";
                        command.AnonimKullanimVerisiPaylasimiIzni = formData["shareUsageData"] == "on";
                        command.ProfilGorunurlugu = formData["profileVisibility"] == "on";
                        break;
                        
                    case "appearance":
                        command.Tema = formData["theme"];
                        command.PanelDuzeni = formData["dashboardLayout"];
                        command.RenkSemasi = formData["colorScheme"];
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
                _logger.LogError(ex, "Ayarlar güncellenirken bir hata oluştu");
                TempData["ErrorMessage"] = "Ayarlar güncellenirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("Settings", new { tab = settingType });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateRandevuCommand command)
        {
            try
            {
                Console.WriteLine("CreateAppointment metodu çağrıldı");
                Console.WriteLine($"Alınan veri: HastaId={command.HastaId}, RandevuBaslangicTarihi={command.RandevuBaslangicTarihi}, RandevuBitisTarihi={command.RandevuBitisTarihi}");
                
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    Console.WriteLine("Oturum bilgileri geçersiz");
                    return Json(new { success = false, message = "Oturum bilgileriniz geçersiz." });
                }

                // Güvenlik kontrolü: Sadece kendi adına randevu oluşturabilir
                command.DiyetisyenId = diyetisyenId;
                Console.WriteLine($"DiyetisyenId atandı: {diyetisyenId}");
                
                // Randevu oluştur
                await _mediator.Send(command);
                Console.WriteLine("Randevu başarıyla oluşturuldu");
                
                return Json(new { success = true, message = "Randevu başarıyla oluşturuldu." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata oluştu: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new { success = false, message = "Randevu oluşturulurken bir hata oluştu: " + ex.Message });
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAppointmentStatus([FromBody] UpdateAppointmentStatusViewModel model)
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return Json(new { success = false, message = "Oturum bilgileriniz geçersiz." });
                }

                // Randevu bilgilerini getir
                var randevu = await _mediator.Send(new GetRandevuByIdQuery(model.Id));
                
                // Güvenlik kontrolü: Sadece kendi randevularını güncelleyebilir
                if (randevu.DiyetisyenId != diyetisyenId)
                {
                    return Json(new { success = false, message = "Bu randevuyu güncelleme yetkiniz yok." });
                }
                
                // Randevu durumunu güncelle
                var updateCommand = new UpdateRandevuCommand
                {
                    Id = model.Id,
                    HastaId = randevu.HastaId,
                    DiyetisyenId = randevu.DiyetisyenId,
                    RandevuBaslangicTarihi = randevu.RandevuBaslangicTarihi,
                    RandevuBitisTarihi = randevu.RandevuBitisTarihi,
                    RandevuTuru = randevu.RandevuTuru,
                    Durum = model.Status,
                    DiyetisyenOnayi = model.Status == "Onaylandı" ? true : randevu.DiyetisyenOnayi,
                    HastaOnayi = randevu.HastaOnayi,
                    Notlar = randevu.Notlar
                };
                
                await _mediator.Send(updateCommand);
                
                return Json(new { success = true, message = "Randevu durumu başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Randevu durumu güncellenirken bir hata oluştu: " + ex.Message });
            }
        }
        
        // ViewModel for the UpdateAppointmentStatus endpoint
        public class UpdateAppointmentStatusViewModel
        {
            public Guid Id { get; set; }
            public string Status { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> GetAppointmentDetails(Guid id)
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return Json(new { success = false, message = "Oturum bilgileriniz geçersiz." });
                }

                // Randevu bilgilerini getir
                var randevu = await _mediator.Send(new GetRandevuByIdQuery(id));
                
                // Güvenlik kontrolü: Sadece kendi randevularını görüntüleyebilir
                if (randevu.DiyetisyenId != diyetisyenId)
                {
                    return Json(new { success = false, message = "Bu randevuyu görüntüleme yetkiniz yok." });
                }
                
                return Json(new { success = true, data = randevu });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Randevu detayları getirilirken bir hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignPatientToDietitian([FromBody] AssignPatientViewModel model)
        {
            try
            {
                // Model geçerlilik kontrolü
                if (model == null || model.hastaId == Guid.Empty)
                {
                    return Json(new { success = false, message = "Geçersiz hasta ID'si. Lütfen tekrar deneyiniz." });
                }

                var hastaId = model.hastaId;
                
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return Json(new { success = false, message = "Oturum bilgileriniz geçersiz." });
                }

                // Önce mevcut hastayı getir
                var hasta = await _mediator.Send(new GetHastaByIdQuery(hastaId));
                if (hasta == null)
                {
                    return Json(new { success = false, message = $"ID:{hastaId} olan hasta bulunamadı" });
                }

                // Hasta güncelleme komutu oluştur - tüm gerekli alanları kopyala
                var updateCommand = new UpdateHastaCommand
                {
                    Id = hastaId,
                    TcKimlikNumarasi = hasta.TcKimlikNumarasi,
                    Ad = hasta.Ad,
                    Soyad = hasta.Soyad,
                    Yas = hasta.Yas,
                    Boy = hasta.Boy,
                    Kilo = hasta.Kilo,
                    Email = hasta.Email,
                    Telefon = hasta.Telefon,
                    DiyetisyenId = diyetisyenId, // Yeni diyetisyen ID'si
                    DiyetProgramiId = hasta.DiyetProgramiId,
                    GunlukKaloriIhtiyaci = hasta.GunlukKaloriIhtiyaci
                };
                
                // Hastayı güncelle
                await _mediator.Send(updateCommand);
                
                return Json(new { success = true, message = "Hasta başarıyla size atandı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Hasta atama işlemi sırasında bir hata oluştu: {ex.Message}" });
            }
        }
        
        // ViewModel for the AssignPatientToDietitian endpoint
        public class AssignPatientViewModel
        {
            public Guid hastaId { get; set; }
        }

        [HttpGet]
        [Route("Dietitian/ViewPatient/{id}")]
        public async Task<IActionResult> ViewPatient(Guid id)
        {
            try
            {
                // Get the logged-in dietitian's ID
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Check if user is admin
                bool isAdmin = User.IsInRole("Admin");

                // Get patient details
                var hastaModel = await _mediator.Send(new GetHastaWithDiyetProgramiQuery(id));
                
                // Check if the patient belongs to the current dietitian, is unassigned, or the user is an admin
                if (!isAdmin && hastaModel.DiyetisyenId != diyetisyenId && hastaModel.DiyetisyenId.HasValue)
                {
                    _logger.LogWarning($"Dietitian {diyetisyenId} attempted to view patient {id} who is not assigned to them");
                    TempData["ErrorMessage"] = "Bu hastayı görüntüleme yetkiniz bulunmamaktadır.";
                    return RedirectToAction("DietitianPatients");
                }
                
                // Get dietitian details
                var diyetisyenModel = await _mediator.Send(new GetDiyetisyenByIdQuery(diyetisyenId));
                ViewBag.DiyetisyenModel = diyetisyenModel;
                
                // Get patient's appointments
                var randevular = await _mediator.Send(new GetRandevuByHastaIdQuery(id));
                ViewBag.Randevular = randevular;
                
                // Get diet plans for dropdown
                var diyetProgramlari = await _mediator.Send(new GetDiyetProgramiQuery());
                ViewBag.DiyetProgramlari = diyetProgramlari;
                
                // Add a flag to indicate if the current dietitian can edit this patient
                ViewBag.CanEditPatient = isAdmin || hastaModel.DiyetisyenId == diyetisyenId || !hastaModel.DiyetisyenId.HasValue;
                
                return View(hastaModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error viewing patient details. Patient ID: {id}");
                ViewBag.ErrorMessage = "Hasta bilgileri getirilirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("DietitianPatients");
            }
        }
        
        [HttpGet]
        [Route("Dietitian/EditPatient/{id}")]
        public async Task<IActionResult> EditPatient(Guid id)
        {
            try
            {
                // Get the logged-in dietitian's ID
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Get patient details
                var hastaModel = await _mediator.Send(new GetHastaByIdQuery(id));
                
                // Check if the patient belongs to the current dietitian
                if (hastaModel.DiyetisyenId != diyetisyenId)
                {
                    _logger.LogWarning($"Dietitian {diyetisyenId} attempted to edit patient {id} who is not assigned to them");
                    return RedirectToAction("DietitianPatients");
                }
                
                return View(hastaModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error editing patient. Patient ID: {id}");
                ViewBag.ErrorMessage = "Hasta bilgileri getirilirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("DietitianPatients");
            }
        }
        
        [HttpPost]
        [Route("Dietitian/EditPatient/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPatient(Guid id, UpdateHastaProfileCommand command)
        {
            try
            {
                // Log the received command data for debugging
                _logger.LogInformation($"Received patient update: ID={command.Id}, Ad={command.Ad}, Soyad={command.Soyad}, " +
                    $"Email={command.Email}, DiyetisyenId={command.DiyetisyenId}, TcKimlikNumarasi={command.TcKimlikNumarasi}, " +
                    $"Adres={command.Adres}, KanGrubu={command.KanGrubu}");
                
                // Log model state
                if (!ModelState.IsValid)
                {
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            _logger.LogWarning($"Model error for {state.Key}: {error.ErrorMessage}");
                        }
                    }
                }
                
                // Get the logged-in dietitian's ID
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    _logger.LogWarning("Invalid user session. User ID is empty or not a valid GUID.");
                    return RedirectToAction("Login", "Account");
                }

                // Ensure the ID from the route matches the command
                command.Id = id;
                
                // Set required properties to prevent model binding errors
                if (string.IsNullOrEmpty(command.TcKimlikNumarasi))
                {
                    // Get the current value from database
                    var existingPatient = await _mediator.Send(new GetHastaByIdQuery(id));
                    command.TcKimlikNumarasi = existingPatient.TcKimlikNumarasi;
                }
                
                // Ensure DiyetisyenId is set correctly
                if (command.DiyetisyenId == null || command.DiyetisyenId == Guid.Empty)
                {
                    command.DiyetisyenId = diyetisyenId;
                }
                
                // Get the patient to check if they belong to this dietitian
                var hastaModel = await _mediator.Send(new GetHastaByIdQuery(id));
                
                // Check if the patient belongs to the current dietitian
                if (hastaModel.DiyetisyenId != diyetisyenId)
                {
                    _logger.LogWarning($"Dietitian {diyetisyenId} attempted to update patient {id} who is not assigned to them");
                    return RedirectToAction("DietitianPatients");
                }
                
                // Update the patient
                await _mediator.Send(command);
                _logger.LogInformation($"Successfully updated patient {id}");
                
                // Redirect to the patient details page
                return RedirectToAction("ViewPatient", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating patient. Patient ID: {id}. Exception: {ex.Message}");
                
                // Log the model state errors if any
                if (!ModelState.IsValid)
                {
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            _logger.LogError($"Model error for {state.Key}: {error.ErrorMessage}");
                        }
                    }
                }
                
                TempData["ErrorMessage"] = $"Hasta bilgileri güncellenirken bir hata oluştu: {ex.Message}";
                return RedirectToAction("EditPatient", new { id }); 
            }
        }

        [HttpGet]
        [Route("api/Dietitian/DietPlans")]
        public async Task<IActionResult> GetDietPlansJson()
        {
            try
            {
                // Get the logged-in dietitian's ID
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return Unauthorized(new { success = false, message = "Geçersiz kullanıcı kimliği veya oturum süresi doldu." });
                }

                // Get all diet programs
                var diyetProgramlari = await _mediator.Send(new GetDiyetProgramiQuery());
                
                // Filter for templates created by this dietitian
                var diyetisyenProgramlari = diyetProgramlari
                    .Where(d => d.OlusturanDiyetisyenId == diyetisyenId)
                    .Select(dp => new {
                        id = dp.Id,
                        ad = dp.Ad,
                        aciklama = dp.Aciklama,
                        baslangicTarihi = dp.BaslangicTarihi,
                        bitisTarihi = dp.BitisTarihi
                    })
                    .ToList();
                
                return Json(diyetisyenProgramlari);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Diet plans could not be fetched as JSON");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("Dietitian/GetPatientQuickView/{id}")]
        public async Task<IActionResult> GetPatientQuickView(Guid id)
        {
            try
            {
                // Get the logged-in dietitian's ID
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return Json(new { success = false, message = "Oturum bilgileriniz geçersiz." });
                }

                // Get patient details
                var hastaModel = await _mediator.Send(new GetHastaWithDiyetProgramiQuery(id));
                
                // Check if the patient belongs to the current dietitian or has no dietitian yet
                if (hastaModel.DiyetisyenId != diyetisyenId && hastaModel.DiyetisyenId.HasValue)
                {
                    _logger.LogWarning($"Dietitian {diyetisyenId} attempted to view patient {id} who is assigned to another dietitian");
                    return Json(new { success = false, message = "Bu hastayı görüntüleme yetkiniz yok." });
                }
                
                // Return patient data in a simplified format for the quick view
                var patientData = new
                {
                    id = hastaModel.Id,
                    ad = hastaModel.Ad,
                    soyad = hastaModel.Soyad,
                    email = hastaModel.Email,
                    telefon = hastaModel.Telefon,
                    yas = hastaModel.Yas,
                    cinsiyet = hastaModel.Cinsiyet,
                    boy = hastaModel.Boy,
                    kilo = hastaModel.Kilo,
                    diyetProgramiId = hastaModel.DiyetProgramiId,
                    diyetProgramiAdi = hastaModel.DiyetProgramiAdi,
                    gunlukKaloriIhtiyaci = hastaModel.GunlukKaloriIhtiyaci,
                    diyetisyenId = hastaModel.DiyetisyenId
                };
                
                return Json(new { success = true, patient = patientData });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching quick view data for patient. Patient ID: {id}");
                return Json(new { success = false, message = "Hasta bilgileri getirilirken bir hata oluştu: " + ex.Message });
            }
        }

        [HttpGet]
        [Route("Dietitian/ViewDietPlan/{id}")]
        public async Task<IActionResult> ViewDietPlan(Guid id)
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Diyet programı verilerini getir
                var diyetProgrami = await _mediator.Send(new GetDiyetProgramiByIdQuery(id));
                
                // Diyetisyenin yetkisi var mı kontrol et
                if (diyetProgrami.OlusturanDiyetisyenId != diyetisyenId)
                {
                    ViewBag.ErrorMessage = "Bu diyet programını görüntüleme yetkiniz bulunmamaktadır.";
                    return RedirectToAction("DietPlans");
                }
                
                // Bu diyet programına atanmış hastaları getir
                var tumHastalar = await _mediator.Send(new GetHastasByDiyetisyenIdQuery(diyetisyenId));
                var atanmisHastalar = tumHastalar.Where(h => h.DiyetProgramiId == id).ToList();
                
                // Bu hastaları modele ekle
                if (diyetProgrami.Hastalar == null)
                {
                    diyetProgrami.Hastalar = new List<Dotnet_Dietitian.Application.Features.CQRS.Results.DiyetProgramiResults.HastaDto>();
                    
                    foreach (var hasta in atanmisHastalar)
                    {
                        diyetProgrami.Hastalar.Add(new Dotnet_Dietitian.Application.Features.CQRS.Results.DiyetProgramiResults.HastaDto
                        {
                            Id = hasta.Id,
                            Ad = hasta.Ad,
                            Soyad = hasta.Soyad
                        });
                    }
                }
                
                return View(diyetProgrami);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Diyet programı detayları getirilirken hata oluştu");
                ViewBag.ErrorMessage = "Diyet programı detayları getirilirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("DietPlans");
            }
        }
        
        [HttpGet]
        [Route("Dietitian/EditDietPlan/{id}")]
        public async Task<IActionResult> EditDietPlan(Guid id)
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Diyet programı verilerini getir
                var diyetProgrami = await _mediator.Send(new GetDiyetProgramiByIdQuery(id));
                
                // Diyetisyenin yetkisi var mı kontrol et
                if (diyetProgrami.OlusturanDiyetisyenId != diyetisyenId)
                {
                    ViewBag.ErrorMessage = "Bu diyet programını düzenleme yetkiniz bulunmamaktadır.";
                    return RedirectToAction("DietPlans");
                }
                
                return View(diyetProgrami);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Diyet programı düzenleme sayfası yüklenirken hata oluştu");
                ViewBag.ErrorMessage = "Diyet programı düzenleme sayfası yüklenirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("DietPlans");
            }
        }
        
        [HttpPost]
        [Route("Dietitian/EditDietPlan/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDietPlan(Guid id, [FromBody] UpdateDiyetProgramiCommand command)
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return Unauthorized(new { success = false, message = "Geçersiz kullanıcı kimliği veya oturum süresi doldu." });
                }

                // Diyet programı verilerini getir
                var diyetProgrami = await _mediator.Send(new GetDiyetProgramiByIdQuery(id));
                
                // Diyetisyenin yetkisi var mı kontrol et
                if (diyetProgrami.OlusturanDiyetisyenId != diyetisyenId)
                {
                    return Unauthorized(new { success = false, message = "Bu diyet programını düzenleme yetkiniz bulunmamaktadır." });
                }
                
                // Command nesnesini doğru ID ile güncelle
                command.Id = id;
                
                // Mediator ile güncelleme işlemini gerçekleştir
                await _mediator.Send(command);
                
                return Ok(new { success = true, message = "Diyet programı başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Diyet programı güncellenirken hata oluştu");
                return BadRequest(new { success = false, message = "Diyet programı güncellenirken bir hata oluştu: " + ex.Message });
            }
        }
        
        [HttpGet]
        [Route("Dietitian/AssignDietPlan/{id}")]
        public async Task<IActionResult> AssignDietPlan(Guid id)
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Diyet programı verilerini getir
                var diyetProgrami = await _mediator.Send(new GetDiyetProgramiByIdQuery(id));
                
                // Diyetisyenin yetkisi var mı kontrol et
                if (diyetProgrami.OlusturanDiyetisyenId != diyetisyenId)
                {
                    ViewBag.ErrorMessage = "Bu diyet programını atama yetkiniz bulunmamaktadır.";
                    return RedirectToAction("DietPlans");
                }
                
                // Diyetisyenin hastalarını getir
                var hastalar = await _mediator.Send(new GetHastasByDiyetisyenIdQuery(diyetisyenId));
                
                ViewBag.Hastalar = hastalar;
                ViewBag.DiyetProgrami = diyetProgrami;
                
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Diyet programı atama sayfası yüklenirken hata oluştu");
                ViewBag.ErrorMessage = "Diyet programı atama sayfası yüklenirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("DietPlans");
            }
        }
        
        [HttpPost]
        [Route("Dietitian/AssignDietPlan")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignDietPlan([FromBody] AssignDietPlanViewModel model)
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return Unauthorized(new { success = false, message = "Geçersiz kullanıcı kimliği veya oturum süresi doldu." });
                }

                // Hasta verisini getir
                var hasta = await _mediator.Send(new GetHastaByIdQuery(model.hastaId));
                
                // Diyetisyenin yetkisi var mı kontrol et
                if (hasta.DiyetisyenId != diyetisyenId)
                {
                    return Unauthorized(new { success = false, message = "Bu hastaya diyet programı atama yetkiniz bulunmamaktadır." });
                }
                
                // Hasta güncelleme komutunu oluştur
                var updateHastaCommand = new UpdateHastaCommand
                {
                    Id = model.hastaId,
                    TcKimlikNumarasi = hasta.TcKimlikNumarasi,
                    Ad = hasta.Ad,
                    Soyad = hasta.Soyad,
                    Email = hasta.Email,
                    Telefon = hasta.Telefon,
                    Yas = hasta.Yas,
                    Boy = hasta.Boy,
                    Kilo = hasta.Kilo,
                    DiyetisyenId = diyetisyenId,
                    DiyetProgramiId = model.diyetProgramiId,
                    GunlukKaloriIhtiyaci = hasta.GunlukKaloriIhtiyaci
                };
                  // Mediator ile güncelleme işlemini gerçekleştir
                await _mediator.Send(updateHastaCommand);
                
                // Diyet programı atandıktan sonra otomatik ödeme talebi oluştur
                try
                {
                    var diyetProgrami = await _mediator.Send(new GetDiyetProgramiByIdQuery(model.diyetProgramiId));
                    
                    var createPaymentRequestCommand = new CreatePaymentRequestCommand
                    {
                        HastaId = model.hastaId,
                        DiyetisyenId = diyetisyenId,
                        DiyetProgramiId = model.diyetProgramiId,
                        Tutar = 500.00m, // Varsayılan tutar - bu daha sonra konfigüre edilebilir
                        VadeTarihi = DateTime.Now.AddDays(30), // 30 gün vade
                        Aciklama = $"'{diyetProgrami?.Ad}' diyet programı için ödeme talebi"
                    };
                    
                    await _mediator.Send(createPaymentRequestCommand);
                    _logger.LogInformation($"Diyet programı atama sonrası otomatik ödeme talebi oluşturuldu. Hasta: {model.hastaId}, Diyetisyen: {diyetisyenId}");
                }
                catch (Exception paymentEx)
                {
                    _logger.LogWarning(paymentEx, "Diyet programı başarıyla atandı ancak ödeme talebi oluşturulurken hata oluştu");
                    // Ödeme talebi oluşturulamasa da diyet programı atama işlemi başarılı sayılır
                }
                
                // AJAX isteği için JSON yanıt döndür
                return Ok(new { success = true, message = "Diyet programı hastaya başarıyla atandı ve ödeme talebi oluşturuldu." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Diyet programı hastaya atanırken hata oluştu");
                return BadRequest(new { success = false, message = "Diyet programı hastaya atanırken bir hata oluştu: " + ex.Message });
            }
        }
        
        // ViewModel for the AssignDietPlan endpoint
        public class AssignDietPlanViewModel
        {
            public Guid hastaId { get; set; }
            public Guid diyetProgramiId { get; set; }
        }

        [HttpGet]
        [Route("Dietitian/ArchiveDietPlan/{id}")]
        public async Task<IActionResult> ArchiveDietPlan(Guid id)
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Diyet programı verilerini getir
                var diyetProgrami = await _mediator.Send(new GetDiyetProgramiByIdQuery(id));
                
                // Diyetisyenin yetkisi var mı kontrol et
                if (diyetProgrami.OlusturanDiyetisyenId != diyetisyenId)
                {
                    TempData["ErrorMessage"] = "Bu diyet programını arşivleme yetkiniz bulunmamaktadır.";
                    return RedirectToAction("DietPlans");
                }
                
                // Burada arşivleme işlemi yapılacak (henüz implementasyonu yok)
                // Gerçek uygulamada DiyetProgrami sınıfına ArşivDurumu özelliği eklenebilir
                
                TempData["SuccessMessage"] = "Diyet programı başarıyla arşivlendi.";
                return RedirectToAction("DietPlans");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Diyet programı arşivlenirken hata oluştu");
                TempData["ErrorMessage"] = "Diyet programı arşivlenirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("DietPlans");
            }
        }
    }
}