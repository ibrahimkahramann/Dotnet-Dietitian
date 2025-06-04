using Dotnet_Dietitian.Application.Features.CQRS.Commands.AppUserCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.AppUserHandlers
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, bool>
    {
        private readonly IRepository<Hasta> _hastaRepository;
        private readonly IRepository<Diyetisyen> _diyetisyenRepository;

        public ForgotPasswordCommandHandler(
            IRepository<Hasta> hastaRepository,
            IRepository<Diyetisyen> diyetisyenRepository)
        {
            _hastaRepository = hastaRepository;
            _diyetisyenRepository = diyetisyenRepository;
        }

        public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            // E-posta adresini Hasta tablosunda ara
            var hasta = await _hastaRepository.GetByFilterAsync(h => h.Email == request.Email);
            if (hasta != null)
            {
                // Hasta bulundu, şifre sıfırlama işlemi burada yapılacak
                return true;
            }

            // E-posta adresini Diyetisyen tablosunda ara
            var diyetisyen = await _diyetisyenRepository.GetByFilterAsync(d => d.Email == request.Email);
            if (diyetisyen != null)
            {
                // Diyetisyen bulundu, şifre sıfırlama işlemi burada yapılacak
                return true;
            }

            // E-posta adresi bulunamadı
            return false;
        }
    }
} 