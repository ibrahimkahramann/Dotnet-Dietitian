using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dotnet_Dietitian.Application.Dtos;
using Dotnet_Dietitian.Application.Features.Results.AppUserResults;
using Dotnet_Dietitian.Application.Queries.AppUserQueries;
using Microsoft.IdentityModel.Tokens;

namespace Dotnet_Dietitian.Application.Tools;

public class JwtTokenGenerator
{
    public static TokenResponseDto GenerateToken(GetCheckAppUserQueryResult result)
    {
        var claims = new List<Claim>();
        if(!string.IsNullOrWhiteSpace(result.Role))
            claims.Add(new Claim(ClaimTypes.Role, result.Role));
        
        claims.Add(new Claim(ClaimTypes.NameIdentifier, result.Id.ToString()));

        if (!string.IsNullOrWhiteSpace( result.Username))
            claims.Add(new Claim("Username", result.Username));
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtTokenDefaults.Key));
        
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var expireDate = DateTime.UtcNow.AddMinutes(JwtTokenDefaults.Expire);

        JwtSecurityToken token = new JwtSecurityToken(issuer: JwtTokenDefaults.ValidIssuer,
            audience: JwtTokenDefaults.ValidAudience,
            claims: claims,
            expires: expireDate,
            signingCredentials: signingCredentials);
        
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        return new TokenResponseDto(tokenHandler.WriteToken(token), expireDate);
    }
}