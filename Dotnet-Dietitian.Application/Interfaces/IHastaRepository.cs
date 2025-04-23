using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.Interfaces;

public interface IHastaRepository : IRepository<Hasta>
{
    Task<IReadOnlyList<Hasta>> GetHastasByDiyetisyenIdAsync(Guid diyetisyenId);
    Task<Hasta> GetHastaWithDiyetProgramiAsync(Guid id);
}
