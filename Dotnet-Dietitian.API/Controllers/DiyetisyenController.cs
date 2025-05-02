using Dotnet_Dietitian.Application.Services;
using Dotnet_Dietitian.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet_Dietitian.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DiyetisyenController : ControllerBase
    {
        private readonly IDiyetisyenService _diyetisyenService;
        
        public DiyetisyenController(IDiyetisyenService diyetisyenService)
        {
            _diyetisyenService = diyetisyenService;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Diyetisyen>>> GetAllDiyetisyenler()
        {
            var diyetisyenler = await _diyetisyenService.GetAllDiyetisyenlerAsync();
            return Ok(diyetisyenler);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Diyetisyen>> GetDiyetisyenById(Guid id)
        {
            var diyetisyen = await _diyetisyenService.GetDiyetisyenByIdAsync(id);
            if (diyetisyen == null)
            {
                return NotFound();
            }
            return Ok(diyetisyen);
        }
        
        [HttpPost]
        public async Task<ActionResult<Diyetisyen>> CreateDiyetisyen(Diyetisyen diyetisyen)
        {
            var createdDiyetisyen = await _diyetisyenService.CreateDiyetisyenAsync(diyetisyen);
            return CreatedAtAction(nameof(GetDiyetisyenById), new { id = createdDiyetisyen.Id }, createdDiyetisyen);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiyetisyen(Guid id, Diyetisyen diyetisyen)
        {
            if (id != diyetisyen.Id)
            {
                return BadRequest();
            }
            
            await _diyetisyenService.UpdateDiyetisyenAsync(diyetisyen);
            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiyetisyen(Guid id)
        {
            await _diyetisyenService.DeleteDiyetisyenAsync(id);
            return NoContent();
        }
        
        [HttpGet("bySehir/{sehir}")]
        public async Task<ActionResult<IEnumerable<Diyetisyen>>> GetDiyetisyenlerBySehir(string sehir)
        {
            var diyetisyenler = await _diyetisyenService.GetDiyetisyenlerBySehirAsync(sehir);
            return Ok(diyetisyenler);
        }
        
        [HttpGet("{id}/withHastalar")]
        public async Task<ActionResult<Diyetisyen>> GetDiyetisyenWithHastalar(Guid id)
        {
            var diyetisyen = await _diyetisyenService.GetDiyetisyenWithHastalarAsync(id);
            if (diyetisyen == null)
            {
                return NotFound();
            }
            return Ok(diyetisyen);
        }
    }
}