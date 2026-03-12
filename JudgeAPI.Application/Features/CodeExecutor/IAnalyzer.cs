
namespace JudgeAPI.Services.Submissions
{
    public interface IAnalyzer
    {
        Task<bool> AnalyzeAsync(int submissionId);
    }
}
