using Dotnet_Dietitian.Application.Services;
using Dotnet_Dietitian.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet_Dietitian.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HastaController : ControllerBase
{
    private readonly IHastaService _hastaService;
    
    public HastaController(IHastaService hastaService)
    {
        _hastaService = hastaService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Hasta>>> GetAllHastalar()
    {
        var hastalar = await _hastaService.GetAllHastalarAsync();
        return Ok(hastalar);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Hasta>> GetHastaById(Guid id)
    {
        var hasta = await _hastaService.GetHastaByIdAsync(id);
        if (hasta == null)
        {
            return NotFound();
        }
        return Ok(hasta);
    }
    
    [HttpPost]
    public async Task<ActionResult<Hasta>> CreateHasta(Hasta hasta)
    {
        var createdHasta = await _hastaService.CreateHastaAsync(hasta);
        return CreatedAtAction(nameof(GetHastaById), new { id = createdHasta.Id }, createdHasta);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateHasta(Guid id, Hasta hasta)
    {
        if (id != hasta.Id)
        {
            return BadRequest();
        }
        
        await _hastaService.UpdateHastaAsync(hasta);
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHasta(Guid id)
    {
        await _hastaService.DeleteHastaAsync(id);
        return NoContent();
    }
    
    [HttpGet("byDiyetisyen/{diyetisyenId}")]
    public async Task<ActionResult<IEnumerable<Hasta>>> GetHastasByDiyetisyenId(Guid diyetisyenId)
    {
        var hastalar = await _hastaService.GetHastasByDiyetisyenIdAsync(diyetisyenId);
        return Ok(hastalar);
    }
    
    [HttpGet("{id}/withDiyetProgrami")]
    public async Task<ActionResult<Hasta>> GetHastaWithDiyetProgrami(Guid id)
    {
        var hasta = await _hastaService.GetHastaWithDiyetProgramiAsync(id);
        if (hasta == null)
        {
            return NotFound();
        }
        return Ok(hasta);
    }
}
