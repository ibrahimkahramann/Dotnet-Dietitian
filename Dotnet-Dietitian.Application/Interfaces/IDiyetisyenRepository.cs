using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.Interfaces;

public interface IDiyetisyenRepository : IRepository<Diyetisyen>
{
    Task<IReadOnlyList<Diyetisyen>> GetDiyetisyenlerBySehirAsync(string sehir);
    Task<Diyetisyen> GetDiyetisyenWithHastalarAsync(Guid id);
}
