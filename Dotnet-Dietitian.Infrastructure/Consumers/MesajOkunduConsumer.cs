using Dotnet_Dietitian.Domain.Events;
using Dotnet_Dietitian.Infrastructure.Hubs;
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