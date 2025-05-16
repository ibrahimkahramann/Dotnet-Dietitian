using Dotnet_Dietitian.Application.Features.CQRS.Results.MesajResults;
using MediatR;
using System;
using System.Collections.Generic;

namespace Dotnet_Dietitian.Application.Features.CQRS.Queries.MesajQueries
{
    public class GetUnreadMessagesQuery : IRequest<List<GetMesajQueryResult>>
    {
        public Guid UserId { get; set; }
        public string UserType { get; set; }
        
        public GetUnreadMessagesQuery(Guid userId, string userType)
        {
            UserId = userId;
            UserType = userType;
        }
    }
}