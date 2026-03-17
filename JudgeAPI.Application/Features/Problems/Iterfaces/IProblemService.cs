using JudgeAPI.Models.Problem;
using JudgeAPI.Models.Unit;

namespace JudgeAPI.Application.Features
{
    public interface IProblemService
    {
        Task<ProblemResponseDTO> CreateAsync(ProblemCreateDTO dto);
        Task DeleteProblemAsync(int id);
        Task<ProblemResponseDTO> GetById(int id);
        Task<ProblemResponseDTO> UpdateAsync(ProblemUpdateDTO dto);
    }
}
