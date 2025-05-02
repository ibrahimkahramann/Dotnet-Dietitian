namespace Dotnet_Dietitian.Domain.Entities;

public class AppRole : BaseEntity
{
    public string AppRoleName { get; set; }
    public List<AppUser> AppUsers { get; set; }
}