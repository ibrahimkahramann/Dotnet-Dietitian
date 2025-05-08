using Dotnet_Dietitian.Application.Features.CQRS.Commands.RandevuCommands;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.RandevuQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet_Dietitian.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RandevuController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RandevuController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var values = await _mediator.Send(new GetRandevuQuery());
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var value = await _mediator.Send(new GetRandevuByIdQuery(id));
            return Ok(value);
        }

        [HttpGet("byHasta/{hastaId}")]
        public async Task<IActionResult> GetByHastaId(Guid hastaId)
        {
            var values = await _mediator.Send(new GetRandevuByHastaIdQuery(hastaId));
            return Ok(values);
        }

        [HttpGet("byDiyetisyen/{diyetisyenId}")]
        public async Task<IActionResult> GetByDiyetisyenId(Guid diyetisyenId)
        {
            var values = await _mediator.Send(new GetRandevuByDiyetisyenIdQuery(diyetisyenId));
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRandevuCommand command)
        {
            await _mediator.Send(command);
            return Ok("Randevu başarıyla oluşturuldu");
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateRandevuCommand command)
        {
            await _mediator.Send(command);
            return Ok("Randevu başarıyla güncellendi");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new RemoveRandevuCommand(id));
            return Ok("Randevu başarıyla silindi");
        }

        [HttpPut("updateOnay")]
        public async Task<IActionResult> UpdateOnay(UpdateRandevuOnayCommand command)
        {
            await _mediator.Send(command);
            return Ok("Randevu onay durumu güncellendi");
        }
    }
}