using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.Services;

public class HastaService : IHastaService
{
    private readonly IHastaRepository _hastaRepository;
    
    public HastaService(IHastaRepository hastaRepository)
    {
        _hastaRepository = hastaRepository;
    }
    
    public async Task<IEnumerable<Hasta>> GetAllHastalarAsync()
    {
        return await _hastaRepository.GetAllAsync();
    }
    
    public async Task<Hasta> GetHastaByIdAsync(Guid id)
    {
        return await _hastaRepository.GetByIdAsync(id);
    }
    
    public async Task<Hasta> CreateHastaAsync(Hasta hasta)
    {
        return await _hastaRepository.AddAsync(hasta);
    }
    
    public async Task UpdateHastaAsync(Hasta hasta)
    {
        await _hastaRepository.UpdateAsync(hasta);
    }
    
    public async Task DeleteHastaAsync(Guid id)
    {
        var hasta = await _hastaRepository.GetByIdAsync(id);
        if (hasta != null)
        {
            await _hastaRepository.DeleteAsync(hasta);
        }
    }
    
    public async Task<IEnumerable<Hasta>> GetHastasByDiyetisyenIdAsync(Guid diyetisyenId)
    {
        return await _hastaRepository.GetHastasByDiyetisyenIdAsync(diyetisyenId);
    }
    
    public async Task<Hasta> GetHastaWithDiyetProgramiAsync(Guid id)
    {
        return await _hastaRepository.GetHastaWithDiyetProgramiAsync(id);
    }
}