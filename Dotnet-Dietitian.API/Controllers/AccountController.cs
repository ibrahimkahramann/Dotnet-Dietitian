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
using Dotnet_Dietitian.Persistence.Context;

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
                
                // UserType değerini forma bakarak al
                string userType = Request.Form["userType"].ToString();
                
                // MediatR ile kullanıcı kontrolü
                var result = await _mediator.Send(new GetCheckAppUserQuery
                {
                    Username = model.Username,
                    Password = hashedPassword
                });

                if (result.IsExist)
                {
                    // Usertype kontrolü
                    if (!string.IsNullOrEmpty(userType) && result.Role != userType)
                    {
                        // Kullanıcı tipi eşleşmiyor, hata mesajı göster
                        ModelState.AddModelError(string.Empty, $"Bu hesap {userType} hesabı değil. Lütfen doğru giriş formu kullanın.");
                        return View(model);
                    }

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
                ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre hatalı");
                return View(model);
            }
            catch (Exception ex)
            {
                // Hatayı loglama
                Console.WriteLine($"Login hatası: {ex.Message}");
                
                // Kullanıcıya bir hata mesajı göster ve aynı sayfaya geri dön
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
                
                // Email kontrolü
                bool emailExists = false;
                if (model.UserType == "Hasta")
                {
                    emailExists = await _dbContext.Hastalar.AnyAsync(h => h.Email == model.Email);
                }
                else
                {
                    emailExists = await _dbContext.Diyetisyenler.AnyAsync(d => d.Email == model.Email);
                }
                
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Bu e-posta adresi zaten kullanılıyor.");
                    return View(model);
                }
                
                // TC Kimlik No kontrolü
                bool tcExists = false;
                if (model.UserType == "Hasta")
                {
                    tcExists = await _dbContext.Hastalar.AnyAsync(h => h.TcKimlikNumarasi == model.IdentityNumber);
                }
                else
                {
                    tcExists = await _dbContext.Diyetisyenler.AnyAsync(d => d.TcKimlikNumarasi == model.IdentityNumber);
                }
                
                if (tcExists)
                {
                    ModelState.AddModelError("IdentityNumber", "Bu TC Kimlik Numarası zaten kullanılıyor.");
                    return View(model);
                }
                
                // Telefon kontrolü (eğer telefon numarası girilmişse)
                if (!string.IsNullOrEmpty(model.Phone))
                {
                    bool phoneExists = false;
                    if (model.UserType == "Hasta")
                    {
                        phoneExists = await _dbContext.Hastalar.AnyAsync(h => h.Telefon == model.Phone);
                    }
                    else
                    {
                        phoneExists = await _dbContext.Diyetisyenler.AnyAsync(d => d.Telefon == model.Phone);
                    }
                    
                    if (phoneExists)
                    {
                        ModelState.AddModelError("Phone", "Bu telefon numarası zaten kullanılıyor.");
                        return View(model);
                    }
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
                
                // Tüm entityler için ortak bir ID oluştur
                var userId = Guid.NewGuid();
                
                // Şifreyi hash'le
                string hashedPassword = HashPassword(model.Password);

                // Transaction başlat
                using (var transaction = await _dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // AppUser oluştur
                        var appUser = new AppUser
                        {
                            Id = userId, // Ortak ID kullan
                            Username = model.Username,
                            Password = hashedPassword, // Hash'lenmiş şifre
                            AppRoleId = roleId
                        };

                        await _dbContext.AppUsers.AddAsync(appUser);
                        await _dbContext.SaveChangesAsync();

                        // Kullanıcı tipine göre Hasta veya Diyetisyen kaydı oluştur
                        if (roleType == "Hasta")
                        {
                            var hasta = new Hasta
                            {
                                Id = userId, // AppUser ile aynı ID'yi kullan
                                TcKimlikNumarasi = model.IdentityNumber,
                                Ad = model.FirstName,
                                Soyad = model.LastName,
                                Email = model.Email,
                                Telefon = model.Phone
                            };

                            await _dbContext.Hastalar.AddAsync(hasta);
                            await _dbContext.SaveChangesAsync();
                        }
                        else if (roleType == "Diyetisyen")
                        {
                            var diyetisyen = new Diyetisyen
                            {
                                Id = userId, // AppUser ile aynı ID'yi kullan
                                TcKimlikNumarasi = model.IdentityNumber,
                                Ad = model.FirstName,
                                Soyad = model.LastName,
                                Email = model.Email,
                                Telefon = model.Phone
                            };

                            await _dbContext.Diyetisyenler.AddAsync(diyetisyen);
                            await _dbContext.SaveChangesAsync();
                        }

                        // Transaction'ı commit et
                        await transaction.CommitAsync();

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
                    catch (DbUpdateException ex)
                    {
                        // Hata durumunda transaction'ı rollback et
                        await transaction.RollbackAsync();
                        
                        // Detaylı hata mesajı oluştur
                        var innerException = ex.InnerException?.Message ?? "";
                        string errorMessage = "Veritabanı kaydı sırasında bir hata oluştu.";
                        
                        if (innerException.Contains("IX_Hastalar_Email") || innerException.Contains("IX_Diyetisyenler_Email"))
                        {
                            ModelState.AddModelError("Email", "Bu e-posta adresi zaten kullanılıyor.");
                        }
                        else if (innerException.Contains("IX_Hastalar_TcKimlikNumarasi") || innerException.Contains("IX_Diyetisyenler_TcKimlikNumarasi"))
                        {
                            ModelState.AddModelError("IdentityNumber", "Bu TC Kimlik Numarası zaten kullanılıyor.");
                        }
                        else if (innerException.Contains("IX_Hastalar_Telefon") || innerException.Contains("IX_Diyetisyenler_Telefon"))
                        {
                            ModelState.AddModelError("Phone", "Bu telefon numarası zaten kullanılıyor.");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, $"{errorMessage} Detay: {innerException}");
                        }
                        
                        return View(model);
                    }
                    catch (Exception ex)
                    {
                        // Hata durumunda transaction'ı rollback et
                        await transaction.RollbackAsync();
                        throw new Exception($"Kayıt işlemi sırasında bir hata oluştu: {ex.Message}", ex);
                    }
                }
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