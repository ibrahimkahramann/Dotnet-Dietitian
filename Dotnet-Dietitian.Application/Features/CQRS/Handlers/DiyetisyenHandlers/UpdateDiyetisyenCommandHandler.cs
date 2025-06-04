using Dotnet_Dietitian.Application.Features.CQRS.Commands.DiyetisyenCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.DiyetisyenHandlers
{
    public class UpdateDiyetisyenCommandHandler : IRequestHandler<UpdateDiyetisyenCommand, Unit>
    {
        private readonly IDiyetisyenRepository _repository;

        public UpdateDiyetisyenCommandHandler(IDiyetisyenRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateDiyetisyenCommand request, CancellationToken cancellationToken)
        {
            var diyetisyen = await _repository.GetByIdAsync(request.Id);
            if (diyetisyen == null)
                throw new Exception($"ID:{request.Id} olan diyetisyen bulunamadı");

            diyetisyen.TcKimlikNumarasi = request.TcKimlikNumarasi;
            diyetisyen.Ad = request.Ad;
            diyetisyen.Soyad = request.Soyad;
            diyetisyen.Uzmanlik = request.Uzmanlik;
            diyetisyen.Email = request.Email;
            diyetisyen.Telefon = request.Telefon;
            diyetisyen.MezuniyetOkulu = request.MezuniyetOkulu;
            diyetisyen.DeneyimYili = request.DeneyimYili;
            diyetisyen.Hakkinda = request.Hakkinda;
            diyetisyen.ProfilResmiUrl = request.ProfilResmiUrl;
            diyetisyen.Sehir = request.Sehir;
            diyetisyen.Unvan = request.Unvan;
            diyetisyen.CalistigiKurum = request.CalistigiKurum;
            diyetisyen.LisansNumarasi = request.LisansNumarasi;

            await _repository.UpdateAsync(diyetisyen);
            return Unit.Value;
        }
    }
} 