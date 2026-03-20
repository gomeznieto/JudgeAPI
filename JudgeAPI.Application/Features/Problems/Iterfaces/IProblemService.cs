namespace JudgeAPI.Application.Features.Problems.Iterfaces
{
    public interface IProblemService
    {
        Task<ProblemResponseDTO> CreateAsync(ProblemCreateDTO dto);
        Task DeleteProblemAsync(int id);
        Task<ProblemResponseDTO> GetById(int id);
        Task<ProblemResponseDTO> UpdateAsync(ProblemUpdateDTO dto);
    }
}
