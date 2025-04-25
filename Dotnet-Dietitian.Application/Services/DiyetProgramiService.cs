// DiyetProgramiService.cs
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.Services
{
    public class DiyetProgramiService : IDiyetProgramiService
    {
        private readonly IDiyetProgramiRepository _diyetProgramiRepository;
        
        public DiyetProgramiService(IDiyetProgramiRepository diyetProgramiRepository)
        {
            _diyetProgramiRepository = diyetProgramiRepository;
        }
        
        public async Task<IEnumerable<DiyetProgrami>> GetAllDiyetProgramlariAsync()
        {
            return await _diyetProgramiRepository.GetAllAsync();
        }
        
        public async Task<DiyetProgrami> GetDiyetProgramiByIdAsync(Guid id)
        {
            return await _diyetProgramiRepository.GetByIdAsync(id);
        }
        
        public async Task<DiyetProgrami> CreateDiyetProgramiAsync(DiyetProgrami diyetProgrami)
        {
            return await _diyetProgramiRepository.AddAsync(diyetProgrami);
        }
        
        public async Task UpdateDiyetProgramiAsync(DiyetProgrami diyetProgrami)
        {
            await _diyetProgramiRepository.UpdateAsync(diyetProgrami);
        }
        
        public async Task DeleteDiyetProgramiAsync(Guid id)
        {
            var diyetProgrami = await _diyetProgramiRepository.GetByIdAsync(id);
            if (diyetProgrami != null)
            {
                await _diyetProgramiRepository.DeleteAsync(diyetProgrami);
            }
        }
        
        public async Task<IEnumerable<DiyetProgrami>> GetDiyetProgramiByDiyetisyenIdAsync(Guid diyetisyenId)
        {
            return await _diyetProgramiRepository.GetDiyetProgramiByDiyetisyenIdAsync(diyetisyenId);
        }
        
        public async Task<DiyetProgrami> GetDiyetProgramiWithHastalarAsync(Guid id)
        {
            return await _diyetProgramiRepository.GetDiyetProgramiWithHastalarAsync(id);
        }
    }
}