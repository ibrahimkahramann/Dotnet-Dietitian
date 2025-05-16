using Dotnet_Dietitian.API.Hubs;
using Dotnet_Dietitian.Domain.Events;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Infrastructure.Consumers
{
    public class MesajOkunduConsumer : IConsumer<MesajOkunduEvent>
    {
        private readonly ILogger<MesajOkunduConsumer> _logger;
        private readonly IHubContext<MesajlasmaChatHub> _hubContext;

        public MesajOkunduConsumer(
            ILogger<MesajOkunduConsumer> logger,
            IHubContext<MesajlasmaChatHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<MesajOkunduEvent> context)
        {
            var olay = context.Message;
            
            _logger.LogInformation($"Mesaj okundu: {olay.MesajId}, " +
                                  $"Okuyan: {olay.OkuyanId} ({olay.OkuyanTipi}), " +
                                  $"Zaman: {olay.OkunmaZamani}");
            
            // SignalR üzerinden gönderene bildirim gönder
            // Burada mesajın hangi görüşmeye ait olduğu bilgisi yok
            // Normalde mesaj ID'ye göre veritabanından bulunabilir
            // Ancak basitlik için şimdilik böyle bırakalım
            string userGroupId = $"user-{olay.OkuyanId}";
            
            await _hubContext.Clients.Group(userGroupId).SendAsync("MessageRead", new
            {
                mesajId = olay.MesajId,
                okuyanId = olay.OkuyanId,
                okuyanTipi = olay.OkuyanTipi,
                okunmaZamani = olay.OkunmaZamani
            });
        }
    }
}