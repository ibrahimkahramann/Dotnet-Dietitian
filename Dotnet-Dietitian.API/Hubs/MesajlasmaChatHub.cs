using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.API.Hubs
{
    public class MesajlasmaChatHub : Hub
    {
        // Aktif kullanıcıları takip etmek için
        private static Dictionary<string, string> _baglantiliKullanicilar = new Dictionary<string, string>();
        
        // Bir kullanıcı için özel mesaj gönderme
        public async Task SendPrivateMessage(string userId, string message)
        {
            if (_baglantiliKullanicilar.TryGetValue(userId, out string connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", Context.UserIdentifier, message);
            }
        }
        
        // Bir kullanıcının grup üyelikleri (ör: hasta-diyetisyen görüşmeleri)
        public async Task JoinGroup(string groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
        }
        
        public async Task LeaveGroup(string groupId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
        }
        
        // Gruba mesaj gönderme
        public async Task SendMessageToGroup(string groupId, string message)
        {
            await Clients.Group(groupId).SendAsync("ReceiveGroupMessage", Context.UserIdentifier, message);
        }
        
        // Kullanıcı bağlandığında
        public override async Task OnConnectedAsync()
        {
            // Kullanıcı ID'sini al (JWT token claim'den veya query string'den alabilirsiniz)
            var userId = Context.User?.FindFirst("sub")?.Value ?? Context.ConnectionId;
            
            _baglantiliKullanicilar[userId] = Context.ConnectionId;
            
            await Clients.Others.SendAsync("UserConnected", userId);
            await base.OnConnectedAsync();
        }
        
        // Kullanıcı ayrıldığında
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User?.FindFirst("sub")?.Value ?? Context.ConnectionId;
            
            _baglantiliKullanicilar.Remove(userId);
            
            await Clients.Others.SendAsync("UserDisconnected", userId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}