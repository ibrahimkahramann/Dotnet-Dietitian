using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetProgramiCommands;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetProgramiQueries;

namespace Dotnet_Dietitian.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Diyetisyen")]
    public class DiyetPlanApiController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DiyetPlanApiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("sablon-olustur")]
        public async Task<IActionResult> SablonOlustur(CreateDiyetProgramiCommand command)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return Unauthorized("Geçersiz kullanıcı");
                }

                command.OlusturanDiyetisyenId = diyetisyenId;
                await _mediator.Send(command);

                return Ok(new
                {
                    message = "Diyet plan şablonu başarıyla oluşturuldu",
                    planAdi = command.Ad
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Şablon oluşturulurken hata oluştu: {ex.Message}");
            }
        }

        [HttpGet("sablonlar")]
        public async Task<IActionResult> SablonlariListele()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var diyetisyenId))
                {
                    return Unauthorized("Geçersiz kullanıcı");
                }

                var programlar = await _mediator.Send(new GetDiyetProgramiQuery());
                var diyetisyenSablonlari = programlar.Where(p => p.OlusturanDiyetisyenId == diyetisyenId).ToList();

                return Ok(diyetisyenSablonlari);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Şablonlar getirilirken hata oluştu: {ex.Message}");
            }
        }
    }

    public class DiyetPlanSablonOlusturModel
    {
        public string Ad { get; set; }
        public string Aciklama { get; set; }
        public string Kategori { get; set; }
        public int SureHafta { get; set; }
        public List<string> Etiketler { get; set; }
    }
}