using JudgeAPI.Entities;
using JudgeAPI.Models.Execution;
using Microsoft.EntityFrameworkCore.Storage;

namespace JudgeAPI.Services.Execution
{
    public interface ICodeExecutorService
    {
        Task<ExecutionResult> ExecuteAsync(int submissionId, TestCase test, CompilationResult result);
    }
}
