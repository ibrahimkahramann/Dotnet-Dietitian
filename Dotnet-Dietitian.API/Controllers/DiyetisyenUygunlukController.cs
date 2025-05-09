using Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetisyenUygunlukCommands;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetisyenUygunlukQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet_Dietitian.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiyetisyenUygunlukController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DiyetisyenUygunlukController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var values = await _mediator.Send(new GetDiyetisyenUygunlukQuery());
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var value = await _mediator.Send(new GetDiyetisyenUygunlukByIdQuery(id));
            return Ok(value);
        }

        [HttpGet("byDiyetisyen/{diyetisyenId}")]
        public async Task<IActionResult> GetByDiyetisyenId(Guid diyetisyenId)
        {
            var values = await _mediator.Send(new GetDiyetisyenUygunlukByDiyetisyenIdQuery(diyetisyenId));
            return Ok(values);
        }

        [HttpGet("byTarih")]
        public async Task<IActionResult> GetByTarih([FromQuery] DateTime baslangicTarihi, [FromQuery] DateTime bitisTarihi)
        {
            var values = await _mediator.Send(new GetDiyetisyenUygunlukByTarihQuery(baslangicTarihi, bitisTarihi));
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDiyetisyenUygunlukCommand command)
        {
            await _mediator.Send(command);
            return Ok("Diyetisyen uygunluk bilgisi başarıyla oluşturuldu");
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateDiyetisyenUygunlukCommand command)
        {
            await _mediator.Send(command);
            return Ok("Diyetisyen uygunluk bilgisi başarıyla güncellendi");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new RemoveDiyetisyenUygunlukCommand(id));
            return Ok("Diyetisyen uygunluk bilgisi başarıyla silindi");
        }

        [HttpPut("updateMuayitDurum/{id}")]
        public async Task<IActionResult> UpdateMuayitDurum(Guid id, [FromQuery] bool muayit)
        {
            await _mediator.Send(new UpdateMuayitDurumCommand(id, muayit));
            return Ok("Diyetisyen uygunluk müsait durumu güncellendi");
        }
    }
}