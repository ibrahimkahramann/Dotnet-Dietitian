using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Dotnet_Dietitian.API.Extensions
{
    /// <summary>
    /// Custom user ID provider for SignalR to consistently identify users 
    /// across connections and reconnections
    /// </summary>
    public class UserIdProvider : IUserIdProvider
    {
        /// <summary>
        /// Gets the user ID for a connection from claims
        /// </summary>
        public string GetUserId(HubConnectionContext connection)
        {
            // Try to get the user ID from claims
            var userId = connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            // If no claim found, try to get it from custom claims
            if (string.IsNullOrEmpty(userId))
            {
                userId = connection.User?.FindFirst("sub")?.Value;
            }
            
            // If still no ID, use the connection ID as a fallback
            if (string.IsNullOrEmpty(userId))
            {
                userId = connection.ConnectionId;
            }
            
            return userId;
        }
    }
}
