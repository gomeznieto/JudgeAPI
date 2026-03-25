using JudgeAPI.Application.Features.CodeExecutor.Dtos;
using JudgeAPI.Domain.Entities;

namespace JudgeAPI.Application.Features.CodeExecutor.Interfaces
{
    public interface ICodeExecutorService
    {
        Task<ExecutionResultDTO> ExecuteAsync(int submissionId, TestCase test, CompilationResultDTO result);
    }
}
