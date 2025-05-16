using Dotnet_Dietitian.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Application.Interfaces
{
    public interface IMesajRepository : IRepository<Mesaj>
    {
        Task<IReadOnlyList<Mesaj>> GetConversationAsync(Guid user1Id, string user1Type, Guid user2Id, string user2Type, int count = 50);
        Task<IReadOnlyList<Mesaj>> GetUnreadMessagesAsync(Guid userId, string userType);
        Task<int> MarkAsReadAsync(Guid mesajId);
    }
}