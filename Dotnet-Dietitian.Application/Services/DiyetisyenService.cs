using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.Services;
public class DiyetisyenService : IDiyetisyenService
{
    private readonly IDiyetisyenRepository _diyetisyenRepository;
    
    public DiyetisyenService(IDiyetisyenRepository diyetisyenRepository)
    {
        _diyetisyenRepository = diyetisyenRepository;
    }
    
    public async Task<IEnumerable<Diyetisyen>> GetAllDiyetisyenlerAsync()
    {
        return await _diyetisyenRepository.GetAllAsync();
    }
    
    public async Task<Diyetisyen> GetDiyetisyenByIdAsync(Guid id)
    {
        return await _diyetisyenRepository.GetByIdAsync(id);
    }
    
    public async Task<Diyetisyen> CreateDiyetisyenAsync(Diyetisyen diyetisyen)
    {
        return await _diyetisyenRepository.AddAsync(diyetisyen);
    }
    
    public async Task UpdateDiyetisyenAsync(Diyetisyen diyetisyen)
    {
        await _diyetisyenRepository.UpdateAsync(diyetisyen);
    }
    
    public async Task DeleteDiyetisyenAsync(Guid id)
    {
        var diyetisyen = await _diyetisyenRepository.GetByIdAsync(id);
        if (diyetisyen != null)
        {
            await _diyetisyenRepository.DeleteAsync(diyetisyen);
        }
    }
    
    public async Task<IEnumerable<Diyetisyen>> GetDiyetisyenlerBySehirAsync(string sehir)
    {
        return await _diyetisyenRepository.GetDiyetisyenlerBySehirAsync(sehir);
    }
    
    public async Task<Diyetisyen> GetDiyetisyenWithHastalarAsync(Guid id)
    {
        return await _diyetisyenRepository.GetDiyetisyenWithHastalarAsync(id);
    }
}
