
namespace JudgeAPI.Services.Submissions
{
    public interface ISubmissionAnalyzerService
    {
        Task<bool> AnalyzeAsync(int submissionId);
    }
}
