using Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetProgramiCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Application.TemplatePattern;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
                
                return Ok(new { 
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