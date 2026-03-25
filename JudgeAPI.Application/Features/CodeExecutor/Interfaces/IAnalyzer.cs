namespace JudgeAPI.Application.Features.CodeExecutor.Interfaces
{
    public interface IAnalyzer
    {
        Task<bool> AnalyzeAsync(int submissionId);
    }
}
