using Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetProgramiCommands;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetProgramiQueries;
using Dotnet_Dietitian.Application.TemplatePattern;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Dotnet_Dietitian.API.Models;

namespace Dotnet_Dietitian.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Diyetisyen, Admin")]
    [AutoValidateAntiforgeryToken] // Controller seviyesinde Antiforgery token doğrulama
    public class DietProgramController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly DiyetProgramOlusturucuFactory _programFactory;
        private readonly IRepository<Hasta> _hastaRepository;
        private readonly IAntiforgery _antiforgery;
        private readonly ILogger<DietProgramController> _logger;

        public DietProgramController(
            IMediator mediator,
            DiyetProgramOlusturucuFactory programFactory,
            IRepository<Hasta> hastaRepository,
            IAntiforgery antiforgery,
            ILogger<DietProgramController> logger)
        {
            _mediator = mediator;
            _programFactory = programFactory;
            _hastaRepository = hastaRepository;
            _antiforgery = antiforgery;
            _logger = logger;
        }

        // ViewModel sınıfı - controller ile aynı dosyada tutuyoruz
        public class ProgramOlusturModel
        {
            public Guid HastaId { get; set; }
            public string ProgramTipi { get; set; } // "kilo verme", "kas kazanma", "akdeniz diyeti"
            public int SureGun { get; set; } = 90;
            public string AktiviteSeviyesi { get; set; } = "orta aktif";
        }

        // Kullanıcı kimlik doğrulaması için yardımcı metod
        private bool TryGetDiyetisyenId(out Guid diyetisyenId)
        {
            diyetisyenId = Guid.Empty;
            
            try
            {
                // User identity null kontrolü
                if (User.Identity == null || !User.Identity.IsAuthenticated)
                {
                    _logger.LogWarning("Kullanıcı kimliği doğrulanmamış veya null");
                    return false;
                }
                
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("NameIdentifier claim bulunamadı");
                    return false;
                }
                
                if (!Guid.TryParse(userId, out diyetisyenId))
                {
                    _logger.LogWarning("Geçersiz kullanıcı kimliği formatı: {UserId}", userId);
                    return false;
                }
                
                // Doğrulama başarılı
                _logger.LogInformation("Kullanıcı kimliği doğrulandı: {DiyetisyenId}", diyetisyenId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı kimliği doğrulanırken hata oluştu");
                return false;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var values = await _mediator.Send(new GetDiyetProgramiQuery());
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var value = await _mediator.Send(new GetDiyetProgramiByIdQuery(id));
            return Ok(value);
        }

        [HttpGet("sablonlar")]
        public async Task<IActionResult> GetTemplates()
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                if (!TryGetDiyetisyenId(out var diyetisyenId))
                {
                    return Unauthorized(new ApiErrorResponse 
                    { 
                        StatusCode = 401, 
                        Message = "Geçersiz kullanıcı kimliği" 
                    });
                }

                var programlar = await _mediator.Send(new GetDiyetProgramiQuery());
                var diyetisyenSablonlari = programlar.Where(p => p.OlusturanDiyetisyenId == diyetisyenId).ToList();

                return Ok(diyetisyenSablonlari);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Şablonlar getirilirken hata oluştu");
                return StatusCode(500, new ApiErrorResponse 
                { 
                    StatusCode = 500, 
                    Message = "Şablonlar getirilirken hata oluştu", 
                    Detail = ex.Message 
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Metod seviyesinde tekrar ekleyerek güvenliği artırıyoruz
        public async Task<IActionResult> Create([FromBody] CreateDiyetProgramiCommand command)
        {
            try
            {
                // Kimlik doğrulama bilgilerini logla
                _logger.LogInformation("Create metodu çağrıldı. Kullanıcı kimliği doğrulanıyor...");
                _logger.LogInformation("Authentication Type: {AuthType}", User.Identity?.AuthenticationType ?? "Null");
                _logger.LogInformation("Is Authenticated: {IsAuth}", User.Identity?.IsAuthenticated.ToString() ?? "Null");
                
                if (User.Identity?.IsAuthenticated == true)
                {
                    _logger.LogInformation("Mevcut Claims:");
                    foreach (var claim in User.Claims)
                    {
                        _logger.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
                    }
                }

                // Giriş yapmış kullanıcının ID'sini al
                if (!TryGetDiyetisyenId(out var diyetisyenId))
                {
                    _logger.LogWarning("Kullanıcı kimliği doğrulanamadı - 401 döndürülüyor");
                    return Unauthorized(new ApiErrorResponse 
                    { 
                        StatusCode = 401, 
                        Message = "Geçersiz kullanıcı kimliği veya oturum süresi doldu. Lütfen tekrar giriş yapın." 
                    });
                }

                // Diyetisyen ID'sini command nesnesine ata
                command.OlusturanDiyetisyenId = diyetisyenId;
                
                _logger.LogInformation("Diyet programı oluşturuluyor: {PlanAdi}, Diyetisyen: {DiyetisyenId}", 
                    command.Ad, diyetisyenId);
                    
                await _mediator.Send(command);
                
                _logger.LogInformation("Diyet programı başarıyla oluşturuldu: {PlanAdi}", command.Ad);
                return Ok(new { message = "Diyet programı başarıyla oluşturuldu", success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Diyet programı oluşturulurken hata oluştu: {Message}", ex.Message);
                return StatusCode(500, new ApiErrorResponse 
                { 
                    StatusCode = 500, 
                    Message = "Diyet programı oluşturulurken hata oluştu", 
                    Detail = ex.Message 
                });
            }
        }

        [HttpPost("sablon-olustur")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTemplate([FromBody] CreateDiyetProgramiCommand command)
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                if (!TryGetDiyetisyenId(out var diyetisyenId))
                {
                    return Unauthorized(new ApiErrorResponse 
                    { 
                        StatusCode = 401, 
                        Message = "Geçersiz kullanıcı kimliği" 
                    });
                }

                // Diyetisyen ID'sini command nesnesine ata
                command.OlusturanDiyetisyenId = diyetisyenId;
                
                await _mediator.Send(command);
                
                return Ok(new
                {
                    message = "Diyet plan şablonu başarıyla oluşturuldu",
                    planAdi = command.Ad,
                    success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Şablon oluşturulurken hata oluştu");
                return StatusCode(500, new ApiErrorResponse 
                { 
                    StatusCode = 500, 
                    Message = "Şablon oluşturulurken hata oluştu", 
                    Detail = ex.Message 
                });
            }
        }

        [HttpPost("olustur")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProgramWithTemplate([FromBody] ProgramOlusturModel model)
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                if (!TryGetDiyetisyenId(out var diyetisyenId))
                {
                    return Unauthorized(new ApiErrorResponse 
                    { 
                        StatusCode = 401, 
                        Message = "Geçersiz kullanıcı kimliği" 
                    });
                }

                // Hasta bilgilerini al
                var hasta = await _hastaRepository.GetByIdAsync(model.HastaId);
                if (hasta == null)
                    return NotFound(new ApiErrorResponse 
                    { 
                        StatusCode = 404, 
                        Message = $"ID:{model.HastaId} olan hasta bulunamadı" 
                    });

                // Uygun program oluşturucuyu seç
                var programOlusturucu = _programFactory.GetProgramOlusturucu(model.ProgramTipi);

                // Template method kalıbını kullanarak programı oluştur (diyetisyenId parametresi ile)
                var diyetProgramCommand = programOlusturucu.ProgramOlustur(
                    hasta,
                    diyetisyenId, // Oturum açan diyetisyen ID'sini kullan
                    model.SureGun,
                    model.AktiviteSeviyesi
                );

                // Oluşturulan programı kaydet
                await _mediator.Send(diyetProgramCommand);

                return Ok(new
                {
                    message = $"{diyetProgramCommand.Ad} başarıyla oluşturuldu",
                    program = diyetProgramCommand,
                    success = true
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiErrorResponse 
                { 
                    StatusCode = 400, 
                    Message = ex.Message 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Program oluşturulurken hata oluştu");
                return StatusCode(500, new ApiErrorResponse 
                { 
                    StatusCode = 500, 
                    Message = "Program oluşturulurken hata oluştu", 
                    Detail = ex.Message 
                });
            }
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromBody] UpdateDiyetProgramiCommand command)
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                if (!TryGetDiyetisyenId(out var diyetisyenId))
                {
                    return Unauthorized(new ApiErrorResponse 
                    { 
                        StatusCode = 401, 
                        Message = "Geçersiz kullanıcı kimliği" 
                    });
                }

                // Program verisini getir ve diyetisyen kontrolü yap
                var program = await _mediator.Send(new GetDiyetProgramiByIdQuery(command.Id));
                if (program.OlusturanDiyetisyenId != diyetisyenId)
                {
                    return Unauthorized(new ApiErrorResponse 
                    { 
                        StatusCode = 401, 
                        Message = "Bu programı düzenleme yetkiniz bulunmamaktadır" 
                    });
                }

                await _mediator.Send(command);
                return Ok(new { message = "Diyet programı başarıyla güncellendi", success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Program güncellenirken hata oluştu");
                return StatusCode(500, new ApiErrorResponse 
                { 
                    StatusCode = 500, 
                    Message = "Program güncellenirken hata oluştu", 
                    Detail = ex.Message 
                });
            }
        }

        [HttpDelete("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                // Giriş yapmış kullanıcının ID'sini al
                if (!TryGetDiyetisyenId(out var diyetisyenId))
                {
                    return Unauthorized(new ApiErrorResponse 
                    { 
                        StatusCode = 401, 
                        Message = "Geçersiz kullanıcı kimliği" 
                    });
                }

                // Program verisini getir ve diyetisyen kontrolü yap
                var program = await _mediator.Send(new GetDiyetProgramiByIdQuery(id));
                if (program.OlusturanDiyetisyenId != diyetisyenId)
                {
                    return Unauthorized(new ApiErrorResponse 
                    { 
                        StatusCode = 401, 
                        Message = "Bu programı silme yetkiniz bulunmamaktadır" 
                    });
                }

                await _mediator.Send(new RemoveDiyetProgramiCommand(id));
                return Ok(new { message = "Diyet programı başarıyla silindi", success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Program silinirken hata oluştu");
                return StatusCode(500, new ApiErrorResponse 
                { 
                    StatusCode = 500, 
                    Message = "Program silinirken hata oluştu", 
                    Detail = ex.Message 
                });
            }
        }
    }
}