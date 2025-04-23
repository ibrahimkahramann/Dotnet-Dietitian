using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.Services;
public interface IDiyetisyenService
{
    Task<IEnumerable<Diyetisyen>> GetAllDiyetisyenlerAsync();
    Task<Diyetisyen> GetDiyetisyenByIdAsync(Guid id);
    Task<Diyetisyen> CreateDiyetisyenAsync(Diyetisyen diyetisyen);
    Task UpdateDiyetisyenAsync(Diyetisyen diyetisyen);
    Task DeleteDiyetisyenAsync(Guid id);
    Task<IEnumerable<Diyetisyen>> GetDiyetisyenlerBySehirAsync(string sehir);
    Task<Diyetisyen> GetDiyetisyenWithHastalarAsync(Guid id);
}