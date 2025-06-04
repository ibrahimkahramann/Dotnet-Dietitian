namespace Dotnet_Dietitian.Domain.Entities;

public class IlerlemeOlcum : BaseEntity
{
    public Guid HastaId { get; set; }
    public DateTime OlcumTarihi { get; set; }
    public float Kilo { get; set; }
    public float? BelCevresi { get; set; }
    public float? KalcaCevresi { get; set; }
    public float? GogusCevresi { get; set; }
    public float? KolCevresi { get; set; }
    public float? VucutYagOrani { get; set; }
    public string? Notlar { get; set; }
    
    // Navigation property
    public virtual Hasta Hasta { get; set; }
} 