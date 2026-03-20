using JudgeAPI.Application.Features.Units.Dtos;
using JudgeAPI.Models.Unit;

namespace JudgeAPI.Application.Features.Units.Interfaces
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
