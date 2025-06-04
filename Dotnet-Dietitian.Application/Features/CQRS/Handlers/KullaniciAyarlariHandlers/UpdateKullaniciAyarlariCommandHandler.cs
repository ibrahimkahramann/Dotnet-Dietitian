using Dotnet_Dietitian.Application.Features.CQRS.Commands.KullaniciAyarlariCommands;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Application.Features.CQRS.Handlers.KullaniciAyarlariHandlers
{
    public class UpdateKullaniciAyarlariCommandHandler : IRequestHandler<UpdateKullaniciAyarlariCommand, Unit>
    {
        private readonly IRepository<KullaniciAyarlari> _repository;

        public UpdateKullaniciAyarlariCommandHandler(IRepository<KullaniciAyarlari> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateKullaniciAyarlariCommand request, CancellationToken cancellationToken)
        {
            // Kullanıcının mevcut ayarlarını ara
            var ayarlar = await _repository.GetByFilterAsync(x => 
                x.KullaniciId == request.KullaniciId && 
                x.KullaniciTipi == request.KullaniciTipi
            );

            // Ayarlar yoksa yeni oluştur
            if (ayarlar == null)
            {
                ayarlar = new KullaniciAyarlari
                {
                    KullaniciId = request.KullaniciId,
                    KullaniciTipi = request.KullaniciTipi,
                    SonGuncellemeTarihi = DateTime.Now
                };
            }

            // Ayar tipine göre ilgili alanları güncelle
            switch (request.AyarTipi)
            {
                case "general":
                    UpdateGeneralSettings(ayarlar, request);
                    break;
                case "notifications":
                    UpdateNotificationSettings(ayarlar, request);
                    break;
                case "privacy":
                    UpdatePrivacySettings(ayarlar, request);
                    break;
                case "appearance":
                    UpdateAppearanceSettings(ayarlar, request);
                    break;
                default:
                    throw new ArgumentException("Geçersiz ayar tipi");
            }

            ayarlar.SonGuncellemeTarihi = DateTime.Now;

            // Ayarları kaydet
            if (ayarlar.Id == Guid.Empty)
            {
                await _repository.AddAsync(ayarlar);
            }
            else
            {
                await _repository.UpdateAsync(ayarlar);
            }

            return Unit.Value;
        }

        private void UpdateGeneralSettings(KullaniciAyarlari ayarlar, UpdateKullaniciAyarlariCommand request)
        {
            if (request.Dil != null)
                ayarlar.Dil = request.Dil;
            
            if (request.ZamanDilimi != null)
                ayarlar.ZamanDilimi = request.ZamanDilimi;
            
            if (request.TarihFormati != null)
                ayarlar.TarihFormati = request.TarihFormati;
            
            if (request.OlcuBirimi != null)
                ayarlar.OlcuBirimi = request.OlcuBirimi;
            
            // Diyetisyene özel çalışma saati ayarları
            if (ayarlar.KullaniciTipi == "Diyetisyen")
            {
                if (request.CalismaBaslangicSaati != null && TimeSpan.TryParse(request.CalismaBaslangicSaati, out var baslangicSaati))
                    ayarlar.CalismaBaslangicSaati = baslangicSaati;
                
                if (request.CalismaBitisSaati != null && TimeSpan.TryParse(request.CalismaBitisSaati, out var bitisSaati))
                    ayarlar.CalismaBitisSaati = bitisSaati;
                
                if (request.HaftaSonuCalisma.HasValue)
                    ayarlar.HaftaSonuCalisma = request.HaftaSonuCalisma.Value;
            }
        }

        private void UpdateNotificationSettings(KullaniciAyarlari ayarlar, UpdateKullaniciAyarlariCommand request)
        {
            if (request.EmailRandevuBildirimleri.HasValue)
                ayarlar.EmailRandevuBildirimleri = request.EmailRandevuBildirimleri.Value;
            
            if (request.EmailMesajBildirimleri.HasValue)
                ayarlar.EmailMesajBildirimleri = request.EmailMesajBildirimleri.Value;
            
            if (request.EmailDiyetGuncellemeBildirimleri.HasValue)
                ayarlar.EmailDiyetGuncellemeBildirimleri = request.EmailDiyetGuncellemeBildirimleri.Value;
            
            if (request.EmailPazarlamaBildirimleri.HasValue)
                ayarlar.EmailPazarlamaBildirimleri = request.EmailPazarlamaBildirimleri.Value;
            
            if (request.UygulamaRandevuBildirimleri.HasValue)
                ayarlar.UygulamaRandevuBildirimleri = request.UygulamaRandevuBildirimleri.Value;
            
            if (request.UygulamaMesajBildirimleri.HasValue)
                ayarlar.UygulamaMesajBildirimleri = request.UygulamaMesajBildirimleri.Value;
            
            if (request.UygulamaDiyetGuncellemeBildirimleri.HasValue)
                ayarlar.UygulamaDiyetGuncellemeBildirimleri = request.UygulamaDiyetGuncellemeBildirimleri.Value;
            
            if (request.UygulamaGunlukHatirlatmalar.HasValue)
                ayarlar.UygulamaGunlukHatirlatmalar = request.UygulamaGunlukHatirlatmalar.Value;
            
            // Diyetisyene özel bildirim ayarları
            if (ayarlar.KullaniciTipi == "Diyetisyen")
            {
                if (request.EmailYeniHastaBildirimleri.HasValue)
                    ayarlar.EmailYeniHastaBildirimleri = request.EmailYeniHastaBildirimleri.Value;
                
                if (request.UygulamaYeniHastaBildirimleri.HasValue)
                    ayarlar.UygulamaYeniHastaBildirimleri = request.UygulamaYeniHastaBildirimleri.Value;
            }
        }

        private void UpdatePrivacySettings(KullaniciAyarlari ayarlar, UpdateKullaniciAyarlariCommand request)
        {
            if (request.YeniGirisUyarilari.HasValue)
                ayarlar.YeniGirisUyarilari = request.YeniGirisUyarilari.Value;
            
            if (request.OturumZamanAsimi.HasValue)
                ayarlar.OturumZamanAsimi = request.OturumZamanAsimi.Value;
            
            if (request.AnonimKullanimVerisiPaylasimiIzni.HasValue)
                ayarlar.AnonimKullanimVerisiPaylasimiIzni = request.AnonimKullanimVerisiPaylasimiIzni.Value;
            
            // Hastaya özel gizlilik ayarları
            if (ayarlar.KullaniciTipi == "Hasta")
            {
                if (request.SaglikVerisiPaylasimiIzni.HasValue)
                    ayarlar.SaglikVerisiPaylasimiIzni = request.SaglikVerisiPaylasimiIzni.Value;
                
                if (request.AktiviteVerisiPaylasimiIzni.HasValue)
                    ayarlar.AktiviteVerisiPaylasimiIzni = request.AktiviteVerisiPaylasimiIzni.Value;
            }
            
            // Diyetisyene özel gizlilik ayarları
            if (ayarlar.KullaniciTipi == "Diyetisyen")
            {
                if (request.ProfilGorunurlugu.HasValue)
                    ayarlar.ProfilGorunurlugu = request.ProfilGorunurlugu.Value;
            }
        }

        private void UpdateAppearanceSettings(KullaniciAyarlari ayarlar, UpdateKullaniciAyarlariCommand request)
        {
            if (request.Tema != null)
                ayarlar.Tema = request.Tema;
            
            if (request.PanelDuzeni != null)
                ayarlar.PanelDuzeni = request.PanelDuzeni;
            
            if (request.RenkSemasi != null)
                ayarlar.RenkSemasi = request.RenkSemasi;
            
            // Hastaya özel görünüm ayarları
            if (ayarlar.KullaniciTipi == "Hasta")
            {
                if (request.IlerlemeGrafigiGoster.HasValue)
                    ayarlar.IlerlemeGrafigiGoster = request.IlerlemeGrafigiGoster.Value;
                
                if (request.SuTakibiGoster.HasValue)
                    ayarlar.SuTakibiGoster = request.SuTakibiGoster.Value;
                
                if (request.KaloriTakibiGoster.HasValue)
                    ayarlar.KaloriTakibiGoster = request.KaloriTakibiGoster.Value;
            }
        }
    }
} 