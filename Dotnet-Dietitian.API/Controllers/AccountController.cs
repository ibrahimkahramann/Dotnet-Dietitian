using Microsoft.AspNetCore.Mvc;
using Dotnet_Dietitian.API.Models;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MediatR;
using Dotnet_Dietitian.Domain.Entities;
using Dotnet_Dietitian.Application.Interfaces.AppUserInterfaces;
using System;
using Dotnet_Dietitian.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Dotnet_Dietitian.Application.Features.Results.AppUserResults;
using Dotnet_Dietitian.Application.Dtos;
using System.Text;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Dotnet_Dietitian.Persistence.Context;
using Dotnet_Dietitian.Application.Features.CQRS.Commands.AppUserCommands;

namespace Dotnet_Dietitian.API.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IAppUserRepository _appUserRepository;
        private readonly IRepository<AppRole> _appRoleRepository;
        private readonly IRepository<Hasta> _hastaRepository;
        private readonly IRepository<Diyetisyen> _diyetisyenRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ApplicationDbContext _dbContext;

        public AccountController(
            IMediator mediator, 
            IAppUserRepository appUserRepository,
            IRepository<AppRole> appRoleRepository,
            IRepository<Hasta> hastaRepository,
            IRepository<Diyetisyen> diyetisyenRepository,
            IJwtTokenGenerator jwtTokenGenerator,
            ApplicationDbContext dbContext)
        {
            _mediator = mediator;
            _appUserRepository = appUserRepository;
            _appRoleRepository = appRoleRepository;
            _hastaRepository = hastaRepository;
            _diyetisyenRepository = diyetisyenRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginCommand());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginCommand command, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(command);
            }

            try
            {
                // UserType değerini formdan al
                command.UserType = Request.Form["userType"].ToString();

                // CQRS ile login işlemi
                var result = await _mediator.Send(command);

                if (result.IsExist)
                {
                    // JWT token oluştur
                    var tokenResponse = _jwtTokenGenerator.GenerateToken(result);
                    
                    // User.Identity üzerinden kullanabilmek için claim-based authentication
                    var claimsIdentity = new ClaimsIdentity(
                        new JwtSecurityTokenHandler().ReadJwtToken(tokenResponse.Token).Claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        new AuthenticationProperties
                        {
                            IsPersistent = command.RememberMe,
                            ExpiresUtc = tokenResponse.ExpireDate
                        });

                    // JWT tokeni cookie'ye ekleyebiliriz (opsiyonel)
                    Response.Cookies.Append("jwt_token", tokenResponse.Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = tokenResponse.ExpireDate
                    });

                    // Kullanıcı tipini de sessionStorage için cookie olarak ekle
                    Response.Cookies.Append("userType", result.Role, new CookieOptions
                    {
                        HttpOnly = false, // JavaScript erişimi için false
                        Secure = true,
                        SameSite = SameSiteMode.Lax,
                        Expires = tokenResponse.ExpireDate
                    });

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    // Kullanıcı tipine göre yönlendirme yap
                    if (result.Role == "Hasta")
                    {
                        return RedirectToAction("Dashboard", "Patient");
                    }
                    else if (result.Role == "Diyetisyen")
                    {
                        return RedirectToAction("Dashboard", "Dietitian");
                    }
                    
                    return RedirectToAction("Index", "Home");
                }

                // Kullanıcı bulunamadı veya şifre hatalı
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(command);
            }
            catch (Exception ex)
            {
                // Hatayı loglama
                Console.WriteLine($"Login hatası: {ex.Message}");
                
                // Kullanıcıya bir hata mesajı göster ve aynı sayfaya geri dön
                ModelState.AddModelError(string.Empty, $"Giriş işlemi sırasında bir hata oluştu: {ex.Message}");
                return View(command);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterCommand());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterCommand command)
        {
            if (!ModelState.IsValid)
            {
                return View(command);
            }

            try
            {
                // CQRS ile kayıt işlemi
                try
                {
                    var userId = await _mediator.Send(command);
                    TempData["SuccessMessage"] = "Kaydınız başarıyla tamamlandı. Şimdi giriş yapabilirsiniz.";
                    return RedirectToAction(nameof(Login));
                }
                catch (Exception ex)
                {
                    // Handler'dan gelen hata mesajlarını ModelState'e ekle
                    if (ex.Message.Contains("kullanıcı adı"))
                    {
                        ModelState.AddModelError("Username", ex.Message);
                    }
                    else if (ex.Message.Contains("e-posta"))
                    {
                        ModelState.AddModelError("Email", ex.Message);
                    }
                    else if (ex.Message.Contains("TC Kimlik"))
                    {
                        ModelState.AddModelError("IdentityNumber", ex.Message);
                    }
                    else if (ex.Message.Contains("telefon"))
                    {
                        ModelState.AddModelError("Phone", ex.Message);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                    
                    return View(command);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Kayıt sırasında bir hata oluştu: {ex.Message}");
                return View(command);
            }
        }

        public async Task<IActionResult> Logout()
        {
            // Cookie'den JWT token'ı temizle
            Response.Cookies.Delete("jwt_token");
            
            // Cookie authentication sign out
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            return RedirectToAction("Index", "Home");
        }
        
        // Şifre hash fonksiyonu
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}