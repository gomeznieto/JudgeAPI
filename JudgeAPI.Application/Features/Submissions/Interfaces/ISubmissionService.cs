using JudgeAPI.Application.Features.Submissions.Dtos;

namespace JudgeAPI.Application.Features.Submissions.Interfaces
{
    public interface ISubmissionService
    {
        Task<bool> AnalyzeSubmissionAsync(int submissionId);
        Task<SubmissionResponseDTO> CreateSubmissionAsync(string userId, int problemId, SubmissionCreateDTO submissionCreateDTO);
        Task<SubmissionResponseDTO?> GetSubmissionAsync(string userId, int submissionId);
    }
}
