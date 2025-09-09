using JudgeAPI.Models.Problem;
using JudgeAPI.Models.Unit;

namespace JudgeAPI.Services.Problem
{
    public interface IProblemService
    {
        Task<ProblemResponseDTO> CreateAsync(ProblemCreateDTO dto);
        Task DeleteProblemAsync(int id);
        Task<ProblemResponseDTO> GetById(int id);
        Task<UnitWithProblemsDTO> GetUnitWithProblemsAsync(int unitId);
        Task<ProblemResponseDTO> UpdateAsync(ProblemUpdateDTO dto);
    }
}
