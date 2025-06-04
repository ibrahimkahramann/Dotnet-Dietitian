using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace Dotnet_Dietitian.Infrastructure.Hubs
{
    public class MesajlasmaChatHub : Hub
    {
        private readonly ILogger<MesajlasmaChatHub> _logger;
        // Aktif kullanıcıları takip etmek için
        private static Dictionary<string, string> _baglantiliKullanicilar = new Dictionary<string, string>();
        
        public MesajlasmaChatHub(ILogger<MesajlasmaChatHub> logger)
        {
            _logger = logger;
        }
        
        // Bir kullanıcı için özel mesaj gönderme
        public async Task SendPrivateMessage(string userId, string message)
        {
            try 
            {
                // Try to use the user's connection ID if available
                if (_baglantiliKullanicilar.TryGetValue(userId, out string connectionId))
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveMessage", Context.UserIdentifier, message);
                    _logger.LogInformation($"Sent private message to user {userId} via connection {connectionId}");
                }
                else
                {
                    // Fallback to using the user's ID as defined by IUserIdProvider
                    await Clients.User(userId).SendAsync("ReceiveMessage", Context.UserIdentifier, message);
                    _logger.LogInformation($"Sent private message to user {userId} via User method");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending private message to user {userId}");
            }
        }
        
        // Bir kullanıcının grup üyelikleri (ör: hasta-diyetisyen görüşmeleri)
        public async Task JoinGroup(string groupId)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
                _logger.LogInformation($"User {Context.UserIdentifier} joined group {groupId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error joining group {groupId}");
            }
        }
        
        public async Task LeaveGroup(string groupId)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
                _logger.LogInformation($"User {Context.UserIdentifier} left group {groupId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error leaving group {groupId}");
            }
        }
        
        // Gruba mesaj gönderme
        public async Task SendMessageToGroup(string groupId, string message)
        {
            try
            {
                await Clients.Group(groupId).SendAsync("ReceiveGroupMessage", Context.UserIdentifier, message);
                _logger.LogInformation($"User {Context.UserIdentifier} sent message to group {groupId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending message to group {groupId}");
            }
        }
        
        // Kullanıcı bağlandığında
        public override async Task OnConnectedAsync()
        {
            try
            {
                // Get user ID from context or claims
                var userId = Context.UserIdentifier;
                if (string.IsNullOrEmpty(userId))
                {
                    userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                            Context.User?.FindFirst("sub")?.Value ?? 
                            Context.ConnectionId;
                }
                
                _baglantiliKullanicilar[userId] = Context.ConnectionId;
                _logger.LogInformation($"User {userId} connected with connectionId {Context.ConnectionId}");
                
                await Clients.Others.SendAsync("UserConnected", userId);
                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnConnectedAsync");
                await base.OnConnectedAsync();
            }
        }
        
        // Kullanıcı ayrıldığında
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                var userId = Context.UserIdentifier;
                if (string.IsNullOrEmpty(userId))
                {
                    userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                            Context.User?.FindFirst("sub")?.Value ?? 
                            Context.ConnectionId;
                }
                
                _baglantiliKullanicilar.Remove(userId);
                _logger.LogInformation($"User {userId} disconnected. Exception: {exception?.Message}");
                
                await Clients.Others.SendAsync("UserDisconnected", userId);
                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnDisconnectedAsync");
                await base.OnDisconnectedAsync(exception);
            }
        }
    }
}