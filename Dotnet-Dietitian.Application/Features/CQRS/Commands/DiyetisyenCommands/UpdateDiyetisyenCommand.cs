using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetisyenCommands
{
    public class UpdateDiyetisyenCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string TcKimlikNumarasi { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string? Uzmanlik { get; set; }
        public string Email { get; set; }
        public string? Telefon { get; set; }
        public string? MezuniyetOkulu { get; set; }
        public int? DeneyimYili { get; set; }
        public string? Hakkinda { get; set; }
        public string? ProfilResmiUrl { get; set; }
        public string? Sehir { get; set; }
        public string? Unvan { get; set; }
        public string? CalistigiKurum { get; set; }
        public string? LisansNumarasi { get; set; }
    }
}