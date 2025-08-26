using JudgeAPI.Models;

namespace JudgeAPI.Services
{
    public interface ISubmissionService
    {
        Task<SubmissionResponseDTO> CreateSubmissionAsync(string userId, int problemId, SubmissionCreateDTO submissionCreateDTO);
        Task<SubmissionResponseWrapper> GetSubmissionAsync(int id);
    }
}
