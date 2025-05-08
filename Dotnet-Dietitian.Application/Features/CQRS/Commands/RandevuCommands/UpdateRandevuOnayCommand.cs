using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.RandevuCommands
{
    public class UpdateRandevuOnayCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public bool DiyetisyenOnayi { get; set; }
        public bool HastaOnayi { get; set; }
        public string Durum { get; set; }
    }
}