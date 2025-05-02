using Dotnet_Dietitian.Application.Dtos;
using Dotnet_Dietitian.Application.Features.Results.AppUserResults;

namespace Dotnet_Dietitian.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        TokenResponseDto GenerateToken(GetCheckAppUserQueryResult user);
    }
}