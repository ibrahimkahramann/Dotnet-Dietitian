namespace Dotnet_Dietitian.Domain.Entities;

public class AppUser : BaseEntity
{
    public string Username { get; set; }
    public string Password { get; set; }
    public Guid AppRoleId { get; set; }

    public AppRole AppRole { get; set; }
}