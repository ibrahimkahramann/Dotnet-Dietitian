using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.HastaCommands
{
    public class UpdateHastaProfileCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string TcKimlikNumarasi { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public int? Yas { get; set; }
        public float? Boy { get; set; }
        public float? Kilo { get; set; }
        public string Email { get; set; }
        public string? Telefon { get; set; }
        public Guid? DiyetisyenId { get; set; }
        public Guid? DiyetProgramiId { get; set; }
        public int? GunlukKaloriIhtiyaci { get; set; }
        
        // Additional profile fields
        public DateTime? DogumTarihi { get; set; }
        public string? Cinsiyet { get; set; }
        public string? Adres { get; set; }
        public string? KanGrubu { get; set; }
        public string? Alerjiler { get; set; }
        public string? KronikHastaliklar { get; set; }
        public string? KullanilanIlaclar { get; set; }
        public bool SaglikBilgisiPaylasimiIzni { get; set; }
    }
} 