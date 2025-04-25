using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.Interfaces;

public interface IDiyetProgramiRepository : IRepository<DiyetProgrami>
{
    Task<IReadOnlyList<DiyetProgrami>> GetDiyetProgramiByDiyetisyenIdAsync(Guid diyetisyenId);
    Task<DiyetProgrami> GetDiyetProgramiWithHastalarAsync(Guid id);
}
