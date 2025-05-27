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