using System;

namespace Dotnet_Dietitian.Application.Features.CQRS.Results.IlerlemeOlcumResults
{
    public class IlerlemeOlcumDto
    {
        public Guid Id { get; set; }
        public Guid HastaId { get; set; }
        public DateTime OlcumTarihi { get; set; }
        public float Kilo { get; set; }
        public float? BelCevresi { get; set; }
        public float? KalcaCevresi { get; set; }
        public float? GogusCevresi { get; set; }
        public float? KolCevresi { get; set; }
        public float? VucutYagOrani { get; set; }
        public string? Notlar { get; set; }
    }
} 