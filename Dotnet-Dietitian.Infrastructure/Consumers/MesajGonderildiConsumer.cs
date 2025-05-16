using Dotnet_Dietitian.API.Hubs;
using Dotnet_Dietitian.Domain.Events;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Infrastructure.Consumers
{
    public class MesajGonderildiConsumer : IConsumer<MesajGonderildiEvent>
    {
        private readonly ILogger<MesajGonderildiConsumer> _logger;
        private readonly IHubContext<MesajlasmaChatHub> _hubContext;

        public MesajGonderildiConsumer(
            ILogger<MesajGonderildiConsumer> logger,
            IHubContext<MesajlasmaChatHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<MesajGonderildiEvent> context)
        {
            var olay = context.Message;
            
            _logger.LogInformation($"Mesaj gönderildi: {olay.MesajId}, " +
                                  $"Gönderen: {olay.GonderenId} ({olay.GonderenTipi}), " +
                                  $"Alıcı: {olay.AliciId} ({olay.AliciTipi})");
            
            // SignalR üzerinden alıcıya bildirim gönder
            // Burada "user-{id}" formatındaki bir grup adını kullanabilir veya
            // diğer bir tanımlama mekanizması kullanabilirsiniz
            string aliciGrupAdi = $"user-{olay.AliciId}";
            
            await _hubContext.Clients.Group(aliciGrupAdi).SendAsync("NewMessage", new
            {
                mesajId = olay.MesajId,
                gonderenId = olay.GonderenId,
                gonderenTipi = olay.GonderenTipi,
                aliciId = olay.AliciId,
                aliciTipi = olay.AliciTipi,
                gonderimZamani = olay.GonderimZamani
            });
            
            // Ayrıca özel bir görüşme grubu da oluşturabilirsiniz
            string conversationGroupId = GetConversationGroupId(olay.GonderenId, olay.AliciId);
            
            await _hubContext.Clients.Group(conversationGroupId).SendAsync("ConversationUpdated", new
            {
                mesajId = olay.MesajId,
                lastMessage = "Yeni mesaj"
            });
        }
        
        private string GetConversationGroupId(System.Guid userId1, System.Guid userId2)
        {
            // İki ID'nin küçükten büyüğe sıralanmasıyla oluşan bir grup ID'si
            return userId1.CompareTo(userId2) < 0 
                ? $"conv-{userId1}-{userId2}"
                : $"conv-{userId2}-{userId1}";
        }
    }
}