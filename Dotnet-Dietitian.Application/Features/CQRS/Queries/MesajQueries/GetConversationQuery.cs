using Dotnet_Dietitian.Application.Features.CQRS.Results.MesajResults;
using MediatR;
using System;
using System.Collections.Generic;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.MesajQueries
{
    public class GetConversationQuery : IRequest<List<GetMesajQueryResult>>
    {
        public Guid User1Id { get; set; }
        public string User1Type { get; set; }
        public Guid User2Id { get; set; }
        public string User2Type { get; set; }
        public int Count { get; set; } = 50;
        
        public GetConversationQuery(Guid user1Id, string user1Type, Guid user2Id, string user2Type, int count = 50)
        {
            User1Id = user1Id;
            User1Type = user1Type;
            User2Id = user2Id;
            User2Type = user2Type;
            Count = count;
        }
    }
}