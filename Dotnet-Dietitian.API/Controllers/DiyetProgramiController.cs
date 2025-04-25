using Dotnet_Dietitian.Application.Services;
using Dotnet_Dietitian.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet_Dietitian.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiyetProgramiController : ControllerBase
{
    private readonly IDiyetProgramiService _diyetProgramiService;
    
    public DiyetProgramiController(IDiyetProgramiService diyetProgramiService)
    {
        _diyetProgramiService = diyetProgramiService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DiyetProgrami>>> GetAllDiyetProgramlari()
    {
        var diyetProgramlari = await _diyetProgramiService.GetAllDiyetProgramlariAsync();
        return Ok(diyetProgramlari);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<DiyetProgrami>> GetDiyetProgramiById(Guid id)
    {
        var diyetProgrami = await _diyetProgramiService.GetDiyetProgramiByIdAsync(id);
        if (diyetProgrami == null)
        {
            return NotFound();
        }
        return Ok(diyetProgrami);
    }
    
    [HttpPost]
    public async Task<ActionResult<DiyetProgrami>> CreateDiyetProgrami(DiyetProgrami diyetProgrami)
    {
        var createdDiyetProgrami = await _diyetProgramiService.CreateDiyetProgramiAsync(diyetProgrami);
        return CreatedAtAction(nameof(GetDiyetProgramiById), new { id = createdDiyetProgrami.Id }, createdDiyetProgrami);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDiyetProgrami(Guid id, DiyetProgrami diyetProgrami)
    {
        if (id != diyetProgrami.Id)
        {
            return BadRequest();
        }
        
        await _diyetProgramiService.UpdateDiyetProgramiAsync(diyetProgrami);
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDiyetProgrami(Guid id)
    {
        await _diyetProgramiService.DeleteDiyetProgramiAsync(id);
        return NoContent();
    }
    
    [HttpGet("byDiyetisyen/{diyetisyenId}")]
    public async Task<ActionResult<IEnumerable<DiyetProgrami>>> GetDiyetProgramiByDiyetisyenId(Guid diyetisyenId)
    {
        var diyetProgramlari = await _diyetProgramiService.GetDiyetProgramiByDiyetisyenIdAsync(diyetisyenId);
        return Ok(diyetProgramlari);
    }
    
    [HttpGet("{id}/withHastalar")]
    public async Task<ActionResult<DiyetProgrami>> GetDiyetProgramiWithHastalar(Guid id)
    {
        var diyetProgrami = await _diyetProgramiService.GetDiyetProgramiWithHastalarAsync(id);
        if (diyetProgrami == null)
        {
            return NotFound();
        }
        return Ok(diyetProgrami);
    }
}