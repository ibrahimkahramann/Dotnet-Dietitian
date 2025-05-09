using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.HastaCommands
{
    public class CreateHastaCommand : IRequest<Unit>
    {
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
    }
}