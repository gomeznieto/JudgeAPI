using JudgeAPI.Models;

namespace JudgeAPI.Services
{
    public interface IProblemService
    {
        Task<ProblemResponseDTO> CreateAsync(ProblemCreateDTO dto);
        Task<ProblemResponseDTO> GetById(int id);
        Task<UnitWithProblemsDTO> GetUnitWithProblemsAsync(int unitId);
        Task<ProblemResponseDTO> UpdateAsync(ProblemUpdateDTO dto);
    }
}
