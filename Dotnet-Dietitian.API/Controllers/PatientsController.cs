using Dotnet_Dietitian.Application.Features.CQRS.Commands.HastaCommands;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.HastaQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet_Dietitian.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PatientsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var values = await _mediator.Send(new GetHastaQuery());
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var value = await _mediator.Send(new GetHastaByIdQuery(id));
            return Ok(value);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateHastaCommand command)
        {
            await _mediator.Send(command);
            return Ok("Hasta başarıyla oluşturuldu");
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateHastaCommand command)
        {
            await _mediator.Send(command);
            return Ok("Hasta başarıyla güncellendi");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new RemoveHastaCommand(id));
            return Ok("Hasta başarıyla silindi");
        }

        [HttpGet("byDiyetisyen/{diyetisyenId}")]
        public async Task<IActionResult> GetByDiyetisyenId(Guid diyetisyenId)
        {
            var values = await _mediator.Send(new GetHastasByDiyetisyenIdQuery(diyetisyenId));
            return Ok(values);
        }

        [HttpGet("{id}/withDiyetProgrami")]
        public async Task<IActionResult> GetWithDiyetProgrami(Guid id)
        {
            var value = await _mediator.Send(new GetHastaWithDiyetProgramiQuery(id));
            return Ok(value);
        }
    }
}
