using JudgeAPI.Application.Features.TestCases.Dtos;

namespace JudgeAPI.Application.Features.TestCases.Interfaces
{
    public interface ITestCaseService
    {
        Task<TestCaseResponseDTO> CreateTestCaseAsync(int problemId, TestCaseCreateDTO dto);
        Task DeleteTestCaseAsync(int problemId, int id);
        Task<TestCaseResponseDTO> GetTestCaseByIdAsync(int problemId, int id);
        Task<List<TestCaseResponseDTO>> GetTestCasesByProblemIdAsync(int problemId, bool onlySamples = false);
        Task<TestCaseResponseDTO> MoveTestCaseAsync(int problemId, int id, int newProblemId);
        Task<TestCaseResponseDTO> UpdateTestCaseAsync(TestCaseUpdateDTO dto);
    }
}
