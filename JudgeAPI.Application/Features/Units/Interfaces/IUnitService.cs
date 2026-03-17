using JudgeAPI.Models.Unit;

namespace JudgeAPI.Services.Unit
{
    public interface IUnitService
    {
        Task<UnitResponseDTO> CreateAsync(UnitCreateDTO dto);
        Task<List<UnitResponseDTO>> GetAllAsync();
        Task<UnitResponseDTO> GetByIdAsync(int id);
        Task<UnitWithProblemsDTO> GetUnitWithProblemsAsync(int unitId);
        Task<UnitResponseDTO> UpdateAsync(UnitUpdateDTO dto);
    }
}
