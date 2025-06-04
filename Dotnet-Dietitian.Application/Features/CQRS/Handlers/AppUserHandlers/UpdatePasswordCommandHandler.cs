using Dotnet_Dietitian.Application.Features.CQRS.Commands.AppUserCommands;
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
    public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, bool>
    {
        private readonly IAppUserRepository _appUserRepository;

        public UpdatePasswordCommandHandler(IAppUserRepository appUserRepository)
        {
            _appUserRepository = appUserRepository;
        }

        public async Task<bool> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
        {
            // Kullanıcıyı bul
            var user = await _appUserRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            // Mevcut şifreyi kontrol et
            string hashedCurrentPassword = HashPassword(request.CurrentPassword);
            if (user.Password != hashedCurrentPassword)
            {
                throw new Exception("Mevcut şifre hatalı.");
            }

            // Yeni şifreyi hash'le ve güncelle
            user.Password = HashPassword(request.NewPassword);
            await _appUserRepository.UpdateAsync(user);

            return true;
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