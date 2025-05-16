using Dotnet_Dietitian.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Application.Interfaces
{
    public interface IDiyetYonetimFacade
    {
        // Hasta işlemleri
        Task<Hasta> GetHastaWithDetailsAsync(Guid hastaId);
        Task<bool> AtamaYapAsync(Guid hastaId, Guid diyetisyenId, Guid diyetProgramiId);
        
        // Randevu işlemleri
        Task<bool> RandevuOlusturAsync(Guid hastaId, Guid diyetisyenId, DateTime baslangicZamani, TimeSpan sure, string? notlar = null);
        Task<IReadOnlyList<Randevu>> GetHastaGelecekRandevulariAsync(Guid hastaId);
        
        // Diyet programı işlemleri
        Task<DiyetProgrami> GetDiyetProgramiWithHastaDetailsAsync(Guid diyetProgramiId);
        Task<IReadOnlyList<DiyetProgrami>> GetDiyetisyenProgramlariAsync(Guid diyetisyenId);
        
        // Ödeme işlemleri
        Task<bool> OdemeYapAsync(Guid hastaId, decimal tutar, string odemeTuru, string? aciklama = null);
        Task<IReadOnlyList<OdemeBilgisi>> GetHastaOdemeleriAsync(Guid hastaId);
    }
}