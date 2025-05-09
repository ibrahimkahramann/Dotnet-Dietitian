using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.Interfaces
{
    public interface IDiyetisyenUygunlukRepository : IRepository<DiyetisyenUygunluk>
    {
        Task<IReadOnlyList<DiyetisyenUygunluk>> GetMusaitSlotlarByDiyetisyenIdAsync(Guid diyetisyenId);
        Task<IReadOnlyList<DiyetisyenUygunluk>> GetUygunlukByTarihAraligindaAsync(DateTime baslangic, DateTime bitis);
    }
}