using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.Services;

public interface IDiyetProgramiService
{
    Task<IEnumerable<DiyetProgrami>> GetAllDiyetProgramlariAsync();
    Task<DiyetProgrami> GetDiyetProgramiByIdAsync(Guid id);
    Task<DiyetProgrami> CreateDiyetProgramiAsync(DiyetProgrami diyetProgrami);
    Task UpdateDiyetProgramiAsync(DiyetProgrami diyetProgrami);
    Task DeleteDiyetProgramiAsync(Guid id);
    Task<IEnumerable<DiyetProgrami>> GetDiyetProgramiByDiyetisyenIdAsync(Guid diyetisyenId);
    Task<DiyetProgrami> GetDiyetProgramiWithHastalarAsync(Guid id);
}