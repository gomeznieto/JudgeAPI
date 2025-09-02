using JudgeAPI.Models.Execution;

namespace JudgeAPI.Services.Execution
{
    public interface ICodeCompilerService
    {
        Task<CompilationResult> CompileAsync(string code, int submissionId);
    }
}
