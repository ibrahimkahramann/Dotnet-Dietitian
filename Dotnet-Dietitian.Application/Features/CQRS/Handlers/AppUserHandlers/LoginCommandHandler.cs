using Dotnet_Dietitian.Application.Features.CQRS.Commands.AppUserCommands;
using Dotnet_Dietitian.Application.Features.Results.AppUserResults;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Application.Interfaces.AppUserInterfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.AppUserHandlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, GetCheckAppUserQueryResult>
    {
        private readonly IAppUserRepository _appUserRepository;
        private readonly IRepository<AppRole> _appRoleRepository;

        public LoginCommandHandler(
            IAppUserRepository appUserRepository,
            IRepository<AppRole> appRoleRepository)
        {
            _appUserRepository = appUserRepository;
            _appRoleRepository = appRoleRepository;
        }

        public async Task<GetCheckAppUserQueryResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Şifreyi hash'le
            string hashedPassword = HashPassword(request.Password);
            
            // Şifreye göre kullanıcıyı kontrol et
            var user = await _appUserRepository.GetByUsernameAndPasswordAsync(request.Username, hashedPassword);
            
            if (user != null)
            {
                // Usertype kontrolü
                if (!string.IsNullOrEmpty(request.UserType) && user.AppRole.AppRoleName != request.UserType)
                {
                    // Kullanıcı tipi eşleşmiyor
                    return new GetCheckAppUserQueryResult
                    {
                        IsExist = false,
                        ErrorMessage = $"Bu hesap {request.UserType} hesabı değil. Lütfen doğru giriş formu kullanın."
                    };
                }

                return new GetCheckAppUserQueryResult
                {
                    IsExist = true,
                    Username = user.Username,
                    Id = user.Id,
                    Role = user.AppRole.AppRoleName
                };
            }
            
            return new GetCheckAppUserQueryResult
            {
                IsExist = false,
                ErrorMessage = "Kullanıcı adı veya şifre hatalı"
            };
        }

        // Şifre hash fonksiyonu
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
} 