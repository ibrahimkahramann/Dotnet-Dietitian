using Dotnet_Dietitian.Application.Features.CQRS.Commands.OdemeBilgisiCommands;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.OdemeBilgisiQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OdemeBilgisiController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OdemeBilgisiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> OdemeBilgisiListesi()
        {
            var values = await _mediator.Send(new GetOdemeBilgisiQuery());
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOdemeBilgisi(Guid id)
        {
            var value = await _mediator.Send(new GetOdemeBilgisiByIdQuery(id));
            return Ok(value);
        }

        [HttpGet("hasta/{hastaId}")]
        public async Task<IActionResult> GetOdemeBilgisiByHastaId(Guid hastaId)
        {
            var values = await _mediator.Send(new GetOdemeBilgisiByHastaIdQuery(hastaId));
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOdemeBilgisi(CreateOdemeBilgisiCommand command)
        {
            await _mediator.Send(command);
            return Ok("Ödeme bilgisi başarıyla eklendi");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOdemeBilgisi(UpdateOdemeBilgisiCommand command)
        {
            await _mediator.Send(command);
            return Ok("Ödeme bilgisi başarıyla güncellendi");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveOdemeBilgisi(Guid id)
        {
            await _mediator.Send(new RemoveOdemeBilgisiCommand(id));
            return Ok("Ödeme bilgisi başarıyla silindi");
        }
    }
}