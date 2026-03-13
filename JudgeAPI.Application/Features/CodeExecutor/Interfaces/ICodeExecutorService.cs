using JudgeAPI.Entities;
using JudgeAPI.Models.Execution;
using Microsoft.EntityFrameworkCore.Storage;

namespace JudgeAPI.Services.Execution
{
    public interface ICodeExecutorService
    {
        Task<ExecutionResult> ExecuteAsync(int submissionId, Entities.TestCase test, CompilationResult result);
    }
}
