using JudgeAPI.Models;

namespace JudgeAPI.Services
{
    public interface IUnitService
    {
        Task<UnitResponseDTO> CreateAsync(UnitCreateDTO dto);
        Task<List<UnitResponseDTO>> GetAllAsync();
        Task<UnitResponseDTO> GetByIdAsync(int id);
        Task<UnitResponseDTO> UpdateAsync(UnitUpdateDTO dto);
    }
}
