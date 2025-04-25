using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.Services;

public interface IHastaService
{
    Task<IEnumerable<Hasta>> GetAllHastalarAsync();
    Task<Hasta> GetHastaByIdAsync(Guid id);
    Task<Hasta> CreateHastaAsync(Hasta hasta);
    Task UpdateHastaAsync(Hasta hasta);
    Task DeleteHastaAsync(Guid id);
    Task<IEnumerable<Hasta>> GetHastasByDiyetisyenIdAsync(Guid diyetisyenId);
    Task<Hasta> GetHastaWithDiyetProgramiAsync(Guid id);
}
