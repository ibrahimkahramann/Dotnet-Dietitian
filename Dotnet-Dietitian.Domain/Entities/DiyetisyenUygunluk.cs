namespace Dotnet_Dietitian.Domain.Entities;

public class DiyetisyenUygunluk : BaseEntity
{
    public Guid DiyetisyenId { get; set; }
    public DateTime Gun { get; set; }
    public TimeSpan BaslangicSaati { get; set; }
    public TimeSpan BitisSaati { get; set; }
    public string? TekrarTipi { get; set; }
    
    // Navigation properties


    public virtual Diyetisyen Diyetisyen { get; set; }
}
