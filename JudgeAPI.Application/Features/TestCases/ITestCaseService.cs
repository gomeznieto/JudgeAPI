using JudgeAPI.Models.TestCase;

namespace JudgeAPI.Services.TestCase
{
    public interface ITestCaseService
    {
        Task<TestCaseResponseDTO> CreateTestCaseAsync(int problemId, TestCaseCreateDTO create);
        Task DeleteTestCaseAsync(int id);
        Task<TestCaseResponseDTO> GetTestCaseByIdAsync(int problemId, int id);
        Task<List<TestCaseResponseDTO>> GetTestCasesByProblemIdAsync(int problemId, bool onlySamples = false);
        Task<TestCaseResponseDTO> MoveTestCaseAsync(int problemId, int id, int newProblemId);
        Task<TestCaseResponseDTO> UpdateTestCaseAsync(TestCaseUpdateDTO update);
    }
}
