using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.AppUserCommands
{
    public class ForgotPasswordCommand : IRequest<bool>
    {
        [Required(ErrorMessage = "E-posta adresi gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [Display(Name = "E-posta Adresi")]
        public string Email { get; set; } = string.Empty;
    }
} 