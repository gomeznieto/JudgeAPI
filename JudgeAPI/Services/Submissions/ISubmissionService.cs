using JudgeAPI.Models.Submission;

namespace JudgeAPI.Services.Submissions
{
    public interface ISubmissionService
    {
        Task<bool> AnalyzeSubmissionAsync(int id);
        Task<SubmissionResponseDTO> CreateSubmissionAsync(string userId, int problemId, SubmissionCreateDTO submissionCreateDTO);
        Task<SubmissionResponseWrapper> GetSubmissionAsync(int id);
    }
}
