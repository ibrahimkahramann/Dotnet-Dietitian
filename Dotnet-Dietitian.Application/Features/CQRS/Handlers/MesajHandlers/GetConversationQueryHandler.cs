using Dotnet_Dietitian.Application.Features.CQRS.Queries.MesajQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Results.MesajResults;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.MesajHandlers
{
    public class GetConversationQueryHandler : IRequestHandler<GetConversationQuery, List<GetMesajQueryResult>>
    {
        private readonly IMesajRepository _mesajRepository;
        private readonly IRepository<Hasta> _hastaRepository;
        private readonly IRepository<Diyetisyen> _diyetisyenRepository;

        public GetConversationQueryHandler(
            IMesajRepository mesajRepository,
            IRepository<Hasta> hastaRepository,
            IRepository<Diyetisyen> diyetisyenRepository)
        {
            _mesajRepository = mesajRepository;
            _hastaRepository = hastaRepository;
            _diyetisyenRepository = diyetisyenRepository;
        }

        public async Task<List<GetMesajQueryResult>> Handle(GetConversationQuery request, CancellationToken cancellationToken)
        {
            var mesajlar = await _mesajRepository.GetConversationAsync(
                request.User1Id, request.User1Type,
                request.User2Id, request.User2Type,
                request.Count);
                
            // Gönderen ve alıcı adlarını belirle
            var result = new List<GetMesajQueryResult>();
            
            foreach (var mesaj in mesajlar)
            {
                var gonderenAd = "";
                var aliciAd = "";
                
                if (mesaj.GonderenTipi == "Hasta")
                {
                    var hasta = await _hastaRepository.GetByIdAsync(mesaj.GonderenId);
                    if (hasta != null) gonderenAd = $"{hasta.Ad} {hasta.Soyad}";
                }
                else
                {
                    var diyetisyen = await _diyetisyenRepository.GetByIdAsync(mesaj.GonderenId);
                    if (diyetisyen != null) gonderenAd = $"{diyetisyen.Ad} {diyetisyen.Soyad}";
                }
                
                if (mesaj.AliciTipi == "Hasta")
                {
                    var hasta = await _hastaRepository.GetByIdAsync(mesaj.AliciId);
                    if (hasta != null) aliciAd = $"{hasta.Ad} {hasta.Soyad}";
                }
                else
                {
                    var diyetisyen = await _diyetisyenRepository.GetByIdAsync(mesaj.AliciId);
                    if (diyetisyen != null) aliciAd = $"{diyetisyen.Ad} {diyetisyen.Soyad}";
                }
                
                result.Add(new GetMesajQueryResult
                {
                    Id = mesaj.Id,
                    GonderenId = mesaj.GonderenId,
                    GonderenTipi = mesaj.GonderenTipi,
                    GonderenAd = gonderenAd,
                    AliciId = mesaj.AliciId,
                    AliciTipi = mesaj.AliciTipi,
                    AliciAd = aliciAd,
                    Icerik = mesaj.Icerik,
                    GonderimZamani = mesaj.GonderimZamani,
                    Okundu = mesaj.Okundu,
                    OkunmaZamani = mesaj.OkunmaZamani
                });
            }
            
            return result.OrderBy(m => m.GonderimZamani).ToList();
        }
    }
}