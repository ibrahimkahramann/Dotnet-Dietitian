namespace Dotnet_Dietitian.Application.Features.Results.AppUserResults;

public class GetCheckAppUserQueryResult
{
    public Guid Id { get; set; }
    
    public string Username { get; set; }
    
    public string Role { get; set; }
    
    public bool IsExist { get; set; }
    
    public string ErrorMessage { get; set; }
}