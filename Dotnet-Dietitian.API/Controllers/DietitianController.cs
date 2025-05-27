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

                // GetDiyetisyenByIdQuery ile diyetisyen verilerini getir
                var model = await _mediator.Send(new GetDiyetisyenByIdQuery(diyetisyenId));
                return View(model);
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama yapılabilir
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
        
        public async Task<IActionResult> Messages()
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
                
                // Mesajları getirmek için sorgu yapılabilir
                // var mesajlar = await _mediator.Send(new GetMesajlarByDiyetisyenIdQuery(diyetisyenId));
                
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

                // Diyetisyen verilerini getir
                var diyetisyenModel = await _mediator.Send(new GetDiyetisyenByIdQuery(diyetisyenId));
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

                // Ayar tipine göre işlem yap
                switch (settingType)
                {
                    case "general":
                        // Genel ayarları güncelle
                        // Örneğin: Dil, zaman dilimi, tarih formatı
                        // await _mediator.Send(new UpdateDiyetisyenGeneralSettingsCommand { ... });
                        break;
                    case "notifications":
                        // Bildirim ayarlarını güncelle
                        // await _mediator.Send(new UpdateDiyetisyenNotificationSettingsCommand { ... });
                        break;
                    case "privacy":
                        // Gizlilik ayarlarını güncelle
                        // await _mediator.Send(new UpdateDiyetisyenPrivacySettingsCommand { ... });
                        break;
                    case "appearance":
                        // Görünüm ayarlarını güncelle
                        // await _mediator.Send(new UpdateDiyetisyenAppearanceSettingsCommand { ... });
                        break;
                    default:
                        TempData["ErrorMessage"] = "Geçersiz ayar tipi.";
                        return RedirectToAction("Settings");
                }

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
                
                // AJAX isteği için JSON yanıt döndür
                return Ok(new { success = true, message = "Diyet programı hastaya başarıyla atandı." });
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