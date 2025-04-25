namespace Dotnet_Dietitian.Domain.Entities;

public class AppRole
{
    public int AppRoleId { get; set; }
    
    public string AppRoleName { get; set; }
    
    public List<AppUser> AppUsers { get; set; }
    
}