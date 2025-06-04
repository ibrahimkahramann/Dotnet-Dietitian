using Dotnet_Dietitian.Application.Features.CQRS.Queries.KullaniciAyarlariQueries;
using Dotnet_Dietitian.Application.Features.CQRS.Results.KullaniciAyarlariResults;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.KullaniciAyarlariHandlers
{
    public class GetKullaniciAyarlariByKullaniciIdQueryHandler : IRequestHandler<GetKullaniciAyarlariByKullaniciIdQuery, GetKullaniciAyarlariQueryResult>
    {
        private readonly IRepository<KullaniciAyarlari> _repository;

        public GetKullaniciAyarlariByKullaniciIdQueryHandler(IRepository<KullaniciAyarlari> repository)
        {
            _repository = repository;
        }

        public async Task<GetKullaniciAyarlariQueryResult> Handle(GetKullaniciAyarlariByKullaniciIdQuery request, CancellationToken cancellationToken)
        {
            // Kullanıcının ayarlarını ara
            var ayarlar = await _repository.GetByFilterAsync(x => 
                x.KullaniciId == request.KullaniciId && 
                x.KullaniciTipi == request.KullaniciTipi
            );

            // Ayarlar yoksa varsayılan değerlerle yeni bir sonuç döndür
            if (ayarlar == null)
            {
                return new GetKullaniciAyarlariQueryResult
                {
                    KullaniciId = request.KullaniciId,
                    KullaniciTipi = request.KullaniciTipi,
                    // Varsayılan değerler
                    ZamanDilimi = "Europe/Istanbul",
                    TarihFormati = "dd/MM/yyyy",
                    OlcuBirimi = "metric",
                    Tema = "light",
                    PanelDuzeni = "default",
                    // Diğer varsayılan değerler false olarak kalacak
                    SonGuncellemeTarihi = DateTime.Now
                };
            }

            // Ayarları sonuç nesnesine dönüştür
            return new GetKullaniciAyarlariQueryResult
            {
                Id = ayarlar.Id,
                KullaniciId = ayarlar.KullaniciId,
                KullaniciTipi = ayarlar.KullaniciTipi,
                
                // Genel Ayarlar
                Dil = ayarlar.Dil,
                ZamanDilimi = ayarlar.ZamanDilimi,
                TarihFormati = ayarlar.TarihFormati,
                OlcuBirimi = ayarlar.OlcuBirimi,
                
                // Diyetisyene özel çalışma saatleri
                CalismaBaslangicSaati = ayarlar.CalismaBaslangicSaati,
                CalismaBitisSaati = ayarlar.CalismaBitisSaati,
                HaftaSonuCalisma = ayarlar.HaftaSonuCalisma,
                
                // Bildirim Ayarları
                EmailRandevuBildirimleri = ayarlar.EmailRandevuBildirimleri,
                EmailMesajBildirimleri = ayarlar.EmailMesajBildirimleri,
                EmailDiyetGuncellemeBildirimleri = ayarlar.EmailDiyetGuncellemeBildirimleri,
                EmailPazarlamaBildirimleri = ayarlar.EmailPazarlamaBildirimleri,
                
                UygulamaRandevuBildirimleri = ayarlar.UygulamaRandevuBildirimleri,
                UygulamaMesajBildirimleri = ayarlar.UygulamaMesajBildirimleri,
                UygulamaDiyetGuncellemeBildirimleri = ayarlar.UygulamaDiyetGuncellemeBildirimleri,
                UygulamaGunlukHatirlatmalar = ayarlar.UygulamaGunlukHatirlatmalar,
                
                // Diyetisyene özel bildirimler
                EmailYeniHastaBildirimleri = ayarlar.EmailYeniHastaBildirimleri,
                UygulamaYeniHastaBildirimleri = ayarlar.UygulamaYeniHastaBildirimleri,
                
                // Gizlilik Ayarları
                YeniGirisUyarilari = ayarlar.YeniGirisUyarilari,
                OturumZamanAsimi = ayarlar.OturumZamanAsimi,
                SaglikVerisiPaylasimiIzni = ayarlar.SaglikVerisiPaylasimiIzni,
                AktiviteVerisiPaylasimiIzni = ayarlar.AktiviteVerisiPaylasimiIzni,
                AnonimKullanimVerisiPaylasimiIzni = ayarlar.AnonimKullanimVerisiPaylasimiIzni,
                
                // Diyetisyene özel gizlilik ayarları
                ProfilGorunurlugu = ayarlar.ProfilGorunurlugu,
                
                // Görünüm Ayarları
                Tema = ayarlar.Tema,
                PanelDuzeni = ayarlar.PanelDuzeni,
                RenkSemasi = ayarlar.RenkSemasi,
                
                // Hastaya özel görünüm tercihleri
                IlerlemeGrafigiGoster = ayarlar.IlerlemeGrafigiGoster,
                SuTakibiGoster = ayarlar.SuTakibiGoster,
                KaloriTakibiGoster = ayarlar.KaloriTakibiGoster,
                
                SonGuncellemeTarihi = ayarlar.SonGuncellemeTarihi
            };
        }
    }
} 