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
using Microsoft.AspNetCore.Http;
using System.Linq;
using Dotnet_Dietitian.Application.Features.CQRS.Commands.HastaCommands;
using Dotnet_Dietitian.Application.Features.CQRS.Commands.AppUserCommands;

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

                // GetHastaByIdQuery ile hasta verilerini getir
                var model = await _mediator.Send(new GetHastaByIdQuery(hastaId));
                return View(model);
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

                // GetHastaByIdQuery ile hasta verilerini getir
                var hastaModel = await _mediator.Send(new GetHastaByIdQuery(hastaId));
                
                // Eğer hastanın bir diyet programı varsa, diyet programı detaylarını getir
                if (hastaModel.DiyetProgramiId.HasValue)
                {
                    // Burada diyet programı detaylarını getirecek bir query eklenebilir
                    // var diyetProgrami = await _mediator.Send(new GetDiyetProgramiByIdQuery(hastaModel.DiyetProgramiId.Value));
                    // return View(diyetProgrami);
                }
                
                return View(hastaModel);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Diyet programı bilgileri getirilirken bir hata oluştu: " + ex.Message;
                return View();
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
                
                // Randevuları getirmek için sorgu yapılabilir
                // var randevular = await _mediator.Send(new GetRandevusByHastaIdQuery(hastaId, showPast));
                
                ViewData["ShowPast"] = showPast;
                return View(hastaModel);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Randevu bilgileri getirilirken bir hata oluştu: " + ex.Message;
                return View();
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

                // Hasta verilerini getir
                var hastaModel = await _mediator.Send(new GetHastaByIdQuery(hastaId));
                
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

                // Ayar tipine göre işlem yap
                switch (settingType)
                {
                    case "general":
                        // Genel ayarları güncelle
                        // Örneğin: Dil, zaman dilimi, tarih formatı, ölçü birimleri
                        // Bu ayarlar için bir command ve handler oluşturulabilir
                        // await _mediator.Send(new UpdateHastaGeneralSettingsCommand { ... });
                        break;
                    case "notifications":
                        // Bildirim ayarlarını güncelle
                        // Örneğin: E-posta bildirimleri, uygulama bildirimleri
                        // await _mediator.Send(new UpdateHastaNotificationSettingsCommand { ... });
                        break;
                    case "privacy":
                        // Gizlilik ayarlarını güncelle
                        // Örneğin: Veri paylaşımı tercihleri
                        // await _mediator.Send(new UpdateHastaPrivacySettingsCommand { ... });
                        break;
                    case "appearance":
                        // Görünüm ayarlarını güncelle
                        // Örneğin: Tema, panel düzeni, görünüm tercihleri
                        // await _mediator.Send(new UpdateHastaAppearanceSettingsCommand { ... });
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