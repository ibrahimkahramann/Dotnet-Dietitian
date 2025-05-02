namespace Dotnet_Dietitian.Application.Tools
{
    public static class JwtTokenDefaults
    {
        public const string Key = "dietitian_secret_key_1234567890_SECURITY_KEY!@#$";
        public const string ValidIssuer = "https://dietitian-api.com";
        public const string ValidAudience = "https://dietitian-client.com";
        public const int Expire = 60;
    }
}