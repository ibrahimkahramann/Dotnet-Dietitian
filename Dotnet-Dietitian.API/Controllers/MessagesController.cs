using Dotnet_Dietitian.Infrastructure.Hubs;
using Dotnet_Dietitian.Application.Features.CQRS.Commands.MesajCommands;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.MesajQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.DiyetisyenQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.HastaQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHubContext<MesajlasmaChatHub> _hubContext;

        public MessagesController(IMediator mediator, IHubContext<MesajlasmaChatHub> hubContext)
        {
            _mediator = mediator;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] CreateMesajCommand command)
        {
            try
            {
                var mesajId = await _mediator.Send(command);
                
                // SignalR ile anında alıcıya bildirim gönder
                // Bu kısım MassTransit consumer'ı tarafından da yapılabilir
                await _hubContext.Clients.User(command.AliciId.ToString())
                    .SendAsync("ReceiveMessage", new
                    {
                        Id = mesajId,
                        GonderenId = command.GonderenId,
                        GonderenTipi = command.GonderenTipi,
                        Icerik = command.Icerik,
                        GonderimZamani = DateTime.Now
                    });
                
                return Ok(new { mesajId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { hata = ex.Message });
            }
        }

        [HttpGet("conversation")]
        public async Task<IActionResult> GetConversation(
            [FromQuery] Guid user1Id, 
            [FromQuery] string user1Type,
            [FromQuery] Guid user2Id, 
            [FromQuery] string user2Type,
            [FromQuery] int count = 50)
        {
            var query = new GetConversationQuery(user1Id, user1Type, user2Id, user2Type, count);
            var messages = await _mediator.Send(query);
            return Ok(messages);
        }

        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadMessages([FromQuery] Guid userId, [FromQuery] string userType)
        {
            var query = new GetUnreadMessagesQuery(userId, userType);
            var messages = await _mediator.Send(query);
            return Ok(messages);
        }

        [HttpPost("{mesajId}/read")]
        public async Task<IActionResult> MarkAsRead(
            Guid mesajId, 
            [FromQuery] Guid okuyanId,
            [FromQuery] string okuyanTipi)
        {
            var command = new MarkAsReadCommand(mesajId, okuyanId, okuyanTipi);
            await _mediator.Send(command);
            return Ok();
        }

        [HttpGet("contacts/diyetisyen/{diyetisyenId}")]
        public async Task<IActionResult> GetDiyetisyenContacts(Guid diyetisyenId)
        {
            try
            {
                // Diyetisyenin hastalarını getir (mesajlaşabileceği kişiler)
                var hastalar = await _mediator.Send(new GetHastasByDiyetisyenIdQuery(diyetisyenId));
                return Ok(hastalar);
            }
            catch (Exception ex)
            {
                return BadRequest(new { hata = ex.Message });
            }
        }

        [HttpGet("contacts/hasta/{hastaId}")]
        public async Task<IActionResult> GetHastaContacts(Guid hastaId)
        {
            try
            {
                // Hastanın diyetisyenini getir
                var hasta = await _mediator.Send(new GetHastaByIdQuery(hastaId));
                
                if (hasta == null || hasta.DiyetisyenId == null)
                {
                    return Ok(new { message = "Henüz bir diyetisyeniniz bulunmuyor." });
                }
                
                var diyetisyen = await _mediator.Send(new GetDiyetisyenByIdQuery(hasta.DiyetisyenId.Value));
                
                // Diyetisyen bilgilerini düzenle
                var diyetisyenContact = new
                {
                    id = diyetisyen.Id,
                    adSoyad = $"{diyetisyen.Ad} {diyetisyen.Soyad}",
                    unvan = diyetisyen.Unvan
                };
                
                return Ok(diyetisyenContact);
            }
            catch (Exception ex)
            {
                return BadRequest(new { hata = ex.Message });
            }
        }

        [HttpGet("contacts/dietitian/{dietitianId}")]
        public async Task<IActionResult> GetDietitianContacts(Guid dietitianId)
        {
            try
            {
                // Get dietitian's patients (people they can message)
                var query = new GetHastasByDiyetisyenIdQuery(dietitianId);
                var patients = await _mediator.Send(query);
                return Ok(patients);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("contacts/patient/{patientId}")]
        public async Task<IActionResult> GetPatientContacts(Guid patientId)
        {
            try
            {
                // Get patient's dietitian
                var query = new GetHastaByIdQuery(patientId);
                var patient = await _mediator.Send(query);
                
                if (patient == null || patient.DiyetisyenId == null)
                {
                    return Ok(new { message = "You don't have a dietitian assigned yet." });
                }
                
                var dietitianQuery = new GetDiyetisyenByIdQuery(patient.DiyetisyenId.Value);
                var dietitian = await _mediator.Send(dietitianQuery);
                
                // Format dietitian info
                var dietitianContact = new
                {
                    id = dietitian.Id,
                    fullName = $"{dietitian.Ad} {dietitian.Soyad}",
                    title = dietitian.Unvan
                };
                
                return Ok(dietitianContact);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}