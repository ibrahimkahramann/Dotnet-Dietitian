using Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetProgramiCommands;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetProgramiQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet_Dietitian.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiyetProgramiController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DiyetProgramiController(IMediator mediator)
        {
            _mediator = mediator;
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

        [HttpPost]
        public async Task<IActionResult> Create(CreateDiyetProgramiCommand command)
        {
            await _mediator.Send(command);
            return Ok("Diyet programı başarıyla oluşturuldu");
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateDiyetProgramiCommand command)
        {
            await _mediator.Send(command);
            return Ok("Diyet programı başarıyla güncellendi");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new RemoveDiyetProgramiCommand(id));
            return Ok("Diyet programı başarıyla silindi");
        }
    }
}