using Microsoft.AspNetCore.Mvc;
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

namespace Dotnet_Dietitian.API.Controllers
{
    [Authorize(Roles = "Diyetisyen, Admin")]
    public class DietitianController : Controller
    {
        private readonly IMediator _mediator;

        public DietitianController(IMediator mediator)
        {
            _mediator = mediator;
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
        
        public async Task<IActionResult> Patients()
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
                
                ViewBag.Hastalar = hastalar;
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
                
                // Randevuları getirmek için sorgu yap
                var randevular = await _mediator.Send(new GetRandevuByDiyetisyenIdQuery(diyetisyenId));
                
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
                
                // Diyet programlarını getir
                var diyetProgramlari = await _mediator.Send(new GetDiyetProgramiQuery());
                // Diyetisyene ait olanları filtrele
                var diyetisyenProgramlari = diyetProgramlari.Where(d => d.OlusturanDiyetisyenId == diyetisyenId).ToList();
                
                ViewBag.DiyetProgramlari = diyetisyenProgramlari;
                return View(diyetisyenModel);
            }
            catch (Exception ex)
            {
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
    }
} 