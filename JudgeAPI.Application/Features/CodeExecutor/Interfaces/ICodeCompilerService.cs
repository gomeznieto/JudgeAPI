using JudgeAPI.Application.Features.CodeExecutor.Dtos;

namespace JudgeAPI.Application.Features.CodeExecutor.Interfaces
{
    public interface ICodeCompilerService
    {
        Task<CompilationResultDTO> CompileAsync(string code, int submissionId);
    }
}
