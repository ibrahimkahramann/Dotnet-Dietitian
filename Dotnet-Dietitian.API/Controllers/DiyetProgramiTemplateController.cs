using Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetProgramiCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Application.TemplatePattern;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Authorization ekleyin
    public class DiyetProgramiTemplateController : ControllerBase
    {
        private readonly DiyetProgramOlusturucuFactory _programFactory;
        private readonly IRepository<Hasta> _hastaRepository;
        private readonly IMediator _mediator;

        public DiyetProgramiTemplateController(
            DiyetProgramOlusturucuFactory programFactory,
            IRepository<Hasta> hastaRepository,
            IMediator mediator)
        {
            _programFactory = programFactory;
            _hastaRepository = hastaRepository;
            _mediator = mediator;
        }

        [HttpPost("olustur")]
        public async Task<IActionResult> OlusturDiyetProgrami(DiyetProgramOlusturModel model)
        {
            try
            {
                // Hasta bilgilerini al
                var hasta = await _hastaRepository.GetByIdAsync(model.HastaId);
                if (hasta == null)
                    return NotFound($"ID:{model.HastaId} olan hasta bulunamadı");

                // Uygun program oluşturucuyu seç
                var programOlusturucu = _programFactory.GetProgramOlusturucu(model.ProgramTipi);

                // Template method kalıbını kullanarak programı oluştur
                var diyetProgramCommand = programOlusturucu.ProgramOlustur(
                    hasta,
                    model.DiyetisyenId,
                    model.SureGun,
                    model.AktiviteSeviyesi
                );

                // Oluşturulan programı kaydet
                await _mediator.Send(diyetProgramCommand);

                return Ok(new
                {
                    message = $"{diyetProgramCommand.Ad} başarıyla oluşturuldu",
                    program = diyetProgramCommand
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Program oluşturulurken hata oluştu: {ex.Message}");
            }
        }

        [HttpPost("sablon-olustur")]
        public async Task<IActionResult> SablonOlustur([FromBody] CreateDiyetProgramiCommand command)
        {
            try
            {
                // Kullanıcı ID'sini al
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
    }

    public class DiyetProgramOlusturModel
    {
        public Guid HastaId { get; set; }
        public Guid DiyetisyenId { get; set; }
        public string ProgramTipi { get; set; } // "kilo verme", "kas kazanma", "akdeniz diyeti"
        public int SureGun { get; set; } = 90;
        public string AktiviteSeviyesi { get; set; } = "orta aktif";
    }
}