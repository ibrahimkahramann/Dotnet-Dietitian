using Dotnet_Dietitian.Application.Features.Results.AppUserResults;
using MediatR;

namespace Dotnet_Dietitian.Application.Queries.AppUserQueries;

public class GetCheckAppUserQuery : IRequest<GetCheckAppUserQueryResult>
{
    public string Username { get; set; }
     
    public string Password { get; set; }
    
}
