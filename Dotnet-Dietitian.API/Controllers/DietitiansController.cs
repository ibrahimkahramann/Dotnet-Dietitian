using Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetisyenCommands;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetisyenQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet_Dietitian.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DietitiansController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DietitiansController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var values = await _mediator.Send(new GetDiyetisyenQuery());
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var value = await _mediator.Send(new GetDiyetisyenByIdQuery(id));
            return Ok(value);
        }

        [HttpGet("bySehir/{sehir}")]
        public async Task<IActionResult> GetBySehir(string sehir)
        {
            var values = await _mediator.Send(new GetDiyetisyenBySehirQuery(sehir));
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDiyetisyenCommand command)
        {
            await _mediator.Send(command);
            return Ok("Diyetisyen başarıyla oluşturuldu");
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateDiyetisyenCommand command)
        {
            await _mediator.Send(command);
            return Ok("Diyetisyen başarıyla güncellendi");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new RemoveDiyetisyenCommand(id));
            return Ok("Diyetisyen başarıyla silindi");
        }
    }
}