using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using System.Security.Claims;
using Dotnet_Dietitian.Application.Features.CQRS.Commands.PaymentRequestCommands;
using Dotnet_Dietitian.Application.Features.CQRS.Queries.PaymentRequestQueries;
using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.API.Controllers;

public class UpdatePaymentRequestStatusRequest
{
    public Guid Id { get; set; }
    public int Durum { get; set; }
    public string? RedNotu { get; set; }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentRequestController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentRequestController> _logger;

    public PaymentRequestController(IMediator mediator, ILogger<PaymentRequestController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreatePaymentRequest([FromBody] CreatePaymentRequestCommand command)
    {
        try
        {
            var paymentRequestId = await _mediator.Send(command);
            return Ok(new { success = true, paymentRequestId, message = "Ödeme talebi başarıyla oluşturuldu." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ödeme talebi oluşturulurken hata oluştu");
            return BadRequest(new { success = false, message = "Ödeme talebi oluşturulurken bir hata oluştu: " + ex.Message });
        }
    }

    [HttpPut]
    [Route("update-status")]
    public async Task<IActionResult> UpdatePaymentRequestStatus([FromBody] UpdatePaymentRequestStatusCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            if (result)
            {
                return Ok(new { success = true, message = "Ödeme talebi durumu başarıyla güncellendi." });
            }
            return NotFound(new { success = false, message = "Ödeme talebi bulunamadı." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ödeme talebi durumu güncellenirken hata oluştu");
            return BadRequest(new { success = false, message = "Ödeme talebi durumu güncellenirken bir hata oluştu: " + ex.Message });
        }
    }

    [HttpPost]
    [Route("UpdateStatus")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdatePaymentRequestStatusRequest request)
    {
        try
        {
            var command = new UpdatePaymentRequestStatusCommand
            {
                Id = request.Id,
                Durum = (PaymentRequestStatus)request.Durum
            };

            // If rejecting, add the rejection reason
            if (request.Durum == 2 && !string.IsNullOrEmpty(request.RedNotu))
            {
                // We need to update the PaymentRequest entity directly to set RedNotu
                // For now, we'll handle this in the command handler
                command.RedNotu = request.RedNotu;
            }

            var result = await _mediator.Send(command);
            if (result)
            {
                return Ok(new { success = true, message = "Ödeme talebi durumu başarıyla güncellendi." });
            }
            return NotFound(new { success = false, message = "Ödeme talebi bulunamadı." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ödeme talebi durumu güncellenirken hata oluştu");
            return BadRequest(new { success = false, message = "Ödeme talebi durumu güncellenirken bir hata oluştu: " + ex.Message });
        }
    }

    [HttpGet]
    [Route("by-hasta/{hastaId}")]
    public async Task<IActionResult> GetPaymentRequestsByHastaId(Guid hastaId)
    {
        try
        {
            var paymentRequests = await _mediator.Send(new GetPaymentRequestsByHastaIdQuery { HastaId = hastaId });
            return Ok(paymentRequests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hasta ödeme talepleri getirilirken hata oluştu");
            return BadRequest(new { success = false, message = "Ödeme talepleri getirilirken bir hata oluştu: " + ex.Message });
        }
    }

    [HttpGet]
    [Route("by-diyetisyen/{diyetisyenId}")]
    public async Task<IActionResult> GetPaymentRequestsByDiyetisyenId(Guid diyetisyenId)
    {
        try
        {
            var paymentRequests = await _mediator.Send(new GetPaymentRequestsByDiyetisyenIdQuery { DiyetisyenId = diyetisyenId });
            return Ok(paymentRequests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Diyetisyen ödeme talepleri getirilirken hata oluştu");
            return BadRequest(new { success = false, message = "Ödeme talepleri getirilirken bir hata oluştu: " + ex.Message });
        }
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetPaymentRequestById(Guid id)
    {
        try
        {
            var paymentRequest = await _mediator.Send(new GetPaymentRequestByIdQuery { Id = id });
            if (paymentRequest == null)
            {
                return NotFound(new { success = false, message = "Ödeme talebi bulunamadı." });
            }
            return Ok(paymentRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ödeme talebi getirilirken hata oluştu");
            return BadRequest(new { success = false, message = "Ödeme talebi getirilirken bir hata oluştu: " + ex.Message });
        }
    }
}
