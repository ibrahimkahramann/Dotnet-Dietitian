namespace Dotnet_Dietitian.Application.Tools;

public class JwtTokenDefaults
{
    public const string ValidAudience = "https://localhost";
    public const string ValidIssuer = "https://localhost";
    public const string Key = "dietitian_secret";
    public const int Expire = 5;
}