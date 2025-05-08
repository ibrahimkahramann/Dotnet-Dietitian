using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.Interfaces
{
    public interface IRandevuRepository : IRepository<Randevu>
    {
        Task<IReadOnlyList<Randevu>> GetRandevularByTarihAraligindaAsync(DateTime baslangic, DateTime bitis);
        Task<IReadOnlyList<Randevu>> GetOnayBekleyenRandevularAsync();
    }
}