namespace Dotnet_Dietitian.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id {get; set;}
    public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;
}