using Dotnet_Dietitian.Application.Features.CQRS.Commands.AppUserCommands;
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
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Guid>
    {
        private readonly IAppUserRepository _appUserRepository;
        private readonly IRepository<AppRole> _appRoleRepository;
        private readonly IRepository<Hasta> _hastaRepository;
        private readonly IRepository<Diyetisyen> _diyetisyenRepository;

        public RegisterCommandHandler(
            IAppUserRepository appUserRepository,
            IRepository<AppRole> appRoleRepository,
            IRepository<Hasta> hastaRepository,
            IRepository<Diyetisyen> diyetisyenRepository)
        {
            _appUserRepository = appUserRepository;
            _appRoleRepository = appRoleRepository;
            _hastaRepository = hastaRepository;
            _diyetisyenRepository = diyetisyenRepository;
        }

        public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Kullanıcı adı kontrolü
            var existingUser = await _appUserRepository.GetByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                throw new Exception("Bu kullanıcı adı zaten kullanılıyor.");
            }
            
            // Email kontrolü
            bool emailExists = false;
            if (request.UserType == "Hasta")
            {
                var existingHastaEmail = await _hastaRepository.GetByFilterAsync(h => h.Email == request.Email);
                emailExists = existingHastaEmail != null;
            }
            else
            {
                var existingDiyetisyenEmail = await _diyetisyenRepository.GetByFilterAsync(d => d.Email == request.Email);
                emailExists = existingDiyetisyenEmail != null;
            }
            
            if (emailExists)
            {
                throw new Exception("Bu e-posta adresi zaten kullanılıyor.");
            }
            
            // TC Kimlik No kontrolü
            bool tcExists = false;
            if (request.UserType == "Hasta")
            {
                var existingHastaTc = await _hastaRepository.GetByFilterAsync(h => h.TcKimlikNumarasi == request.IdentityNumber);
                tcExists = existingHastaTc != null;
            }
            else
            {
                var existingDiyetisyenTc = await _diyetisyenRepository.GetByFilterAsync(d => d.TcKimlikNumarasi == request.IdentityNumber);
                tcExists = existingDiyetisyenTc != null;
            }
            
            if (tcExists)
            {
                throw new Exception("Bu TC Kimlik Numarası zaten kullanılıyor.");
            }
            
            // Telefon kontrolü (eğer telefon numarası girilmişse)
            if (!string.IsNullOrEmpty(request.Phone))
            {
                bool phoneExists = false;
                if (request.UserType == "Hasta")
                {
                    var existingHastaPhone = await _hastaRepository.GetByFilterAsync(h => h.Telefon == request.Phone);
                    phoneExists = existingHastaPhone != null;
                }
                else
                {
                    var existingDiyetisyenPhone = await _diyetisyenRepository.GetByFilterAsync(d => d.Telefon == request.Phone);
                    phoneExists = existingDiyetisyenPhone != null;
                }
                
                if (phoneExists)
                {
                    throw new Exception("Bu telefon numarası zaten kullanılıyor.");
                }
            }

            // Rol bilgisini al (Hasta veya Diyetisyen)
            var roleType = request.UserType;
            var appRole = await _appRoleRepository.GetByFilterAsync(r => r.AppRoleName == roleType);
            if (appRole == null)
            {
                throw new Exception("Geçersiz kullanıcı tipi.");
            }

            var roleId = appRole.Id;
            
            // Tüm entityler için ortak bir ID oluştur
            var userId = Guid.NewGuid();
            
            // Şifreyi hash'le
            string hashedPassword = HashPassword(request.Password);

            // AppUser oluştur
            var appUser = new AppUser
            {
                Id = userId,
                Username = request.Username,
                Password = hashedPassword,
                AppRoleId = roleId
            };

            await _appUserRepository.AddAsync(appUser);

            // Kullanıcı tipine göre Hasta veya Diyetisyen kaydı oluştur
            if (roleType == "Hasta")
            {
                var hasta = new Hasta
                {
                    Id = userId,
                    TcKimlikNumarasi = request.IdentityNumber,
                    Ad = request.FirstName,
                    Soyad = request.LastName,
                    Email = request.Email,
                    Telefon = request.Phone,
                    DogumTarihi = request.BirthDate,
                    Cinsiyet = request.Gender
                };

                await _hastaRepository.AddAsync(hasta);
            }
            else if (roleType == "Diyetisyen")
            {
                var diyetisyen = new Diyetisyen
                {
                    Id = userId,
                    TcKimlikNumarasi = request.IdentityNumber,
                    Ad = request.FirstName,
                    Soyad = request.LastName,
                    Email = request.Email,
                    Telefon = request.Phone
                };

                await _diyetisyenRepository.AddAsync(diyetisyen);
            }

            return userId;
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