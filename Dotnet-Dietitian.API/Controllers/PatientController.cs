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
        
        public async Task<IActionResult> Settings()
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
    }
}