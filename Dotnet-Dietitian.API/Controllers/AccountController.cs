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
using Dotnet_Dietitian.Application.Queries.AppUserQueries;
using Dotnet_Dietitian.Application.Features.Results.AppUserResults;
using Dotnet_Dietitian.Application.Dtos;
using System.Text;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;

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

        public AccountController(
            IMediator mediator, 
            IAppUserRepository appUserRepository,
            IRepository<AppRole> appRoleRepository,
            IRepository<Hasta> hastaRepository,
            IRepository<Diyetisyen> diyetisyenRepository,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _mediator = mediator;
            _appUserRepository = appUserRepository;
            _appRoleRepository = appRoleRepository;
            _hastaRepository = hastaRepository;
            _diyetisyenRepository = diyetisyenRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Şifreyi hash'le
                string hashedPassword = HashPassword(model.Password);
                
                // MediatR ile kullanıcı kontrolü
                var result = await _mediator.Send(new GetCheckAppUserQuery
                {
                    Username = model.Username,
                    Password = hashedPassword
                });

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
                            IsPersistent = model.RememberMe,
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

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre hatalı");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Giriş işlemi sırasında bir hata oluştu: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Kullanıcı adı kontrolü
                var existingUser = await _appUserRepository.GetByUsernameAsync(model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Bu kullanıcı adı zaten kullanılıyor.");
                    return View(model);
                }

                // Rol bilgisini al (Hasta veya Diyetisyen)
                var roleType = model.UserType;
                var appRole = await _appRoleRepository.GetAsync(r => r.AppRoleName == roleType);
                if (!appRole.Any())
                {
                    ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı tipi.");
                    return View(model);
                }

                var roleId = appRole.First().Id;
                
                // Şifreyi hash'le
                string hashedPassword = HashPassword(model.Password);

                // AppUser oluştur
                var appUser = new AppUser
                {
                    Id = Guid.NewGuid(),
                    Username = model.Username,
                    Password = hashedPassword, // Hash'lenmiş şifre
                    AppRoleId = roleId
                };

                await _appUserRepository.AddAsync(appUser);

                // Kullanıcı tipine göre Hasta veya Diyetisyen kaydı oluştur
                Guid entityId = Guid.NewGuid();
                
                if (roleType == "Hasta")
                {
                    var hasta = new Hasta
                    {
                        Id = entityId,
                        TcKimlikNumarasi = model.IdentityNumber,
                        Ad = model.FirstName,
                        Soyad = model.LastName,
                        Email = model.Email,
                        Telefon = model.Phone
                    };

                    await _hastaRepository.AddAsync(hasta);
                }
                else if (roleType == "Diyetisyen")
                {
                    var diyetisyen = new Diyetisyen
                    {
                        Id = entityId,
                        TcKimlikNumarasi = model.IdentityNumber,
                        Ad = model.FirstName,
                        Soyad = model.LastName,
                        Email = model.Email,
                        Telefon = model.Phone
                    };

                    await _diyetisyenRepository.AddAsync(diyetisyen);
                }

                // Kullanıcı oluşturulduktan sonra hemen login işlemi için bir token oluşturabiliriz
                var userResult = new GetCheckAppUserQueryResult
                {
                    Id = appUser.Id,
                    Username = appUser.Username,
                    Role = roleType,
                    IsExist = true
                };
                
                var tokenResponse = _jwtTokenGenerator.GenerateToken(userResult);

                TempData["SuccessMessage"] = "Kaydınız başarıyla tamamlandı. Şimdi giriş yapabilirsiniz.";
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Kayıt sırasında bir hata oluştu: {ex.Message}");
                return View(model);
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