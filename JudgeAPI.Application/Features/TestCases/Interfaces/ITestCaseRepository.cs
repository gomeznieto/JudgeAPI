using JudgeAPI.Application.Common.Interfaces;
using JudgeAPI.Domain.Entities;

namespace JudgeAPI.Application.Features.TestCases.Interfaces
{
    public interface ITestCaseRepository : IRepository<TestCase>
    {
        Task<IList<TestCase>> GetTestCasesAsync(int problemId, bool onlySamples = false, CancellationToken cancellationToken = default);
        Task<TestCase?> GetTestCaseByIdAsync(int problemId, int id, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(int id);
    }
}
