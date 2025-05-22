using Dotnet_Dietitian.Application.Features.Results.AppUserResults;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.AppUserCommands
{
    public class LoginCommand : IRequest<GetCheckAppUserQueryResult>
    {
        [Required(ErrorMessage = "Kullanıcı adı gereklidir")]
        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Şifre gereklidir")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = string.Empty;
        
        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
        
        public string UserType { get; set; } = string.Empty;
    }
} 