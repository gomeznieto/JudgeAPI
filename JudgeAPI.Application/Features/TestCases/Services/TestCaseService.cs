using AutoMapper;
using JudgeAPI.Application.Common;
using JudgeAPI.Application.Common.Interfaces;
using JudgeAPI.Application.Features.Problems.Iterfaces;
using JudgeAPI.Application.Features.TestCases.Dtos;
using JudgeAPI.Application.Features.TestCases.Interfaces;
using JudgeAPI.Domain.Entities;

namespace JudgeAPI.Application.Features.TestCases.Services
{
    public class TestCaseService(
            IMapper mapper,
            ITestCaseRepository testCaseRepository,
            IProblemRepository problemRepository,
            IUnitOfWork unitOfWork
            )
        : ITestCaseService
    {
        private readonly IMapper _mapper = mapper;
        private readonly ITestCaseRepository _testCaseRepository = testCaseRepository;
        private readonly IProblemRepository _problemRepository = problemRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        // ---  CREATE TEST CASE ---
        public async Task<TestCaseResponseDTO> CreateTestCaseAsync(int problemId, TestCaseCreateDTO dto)
        {
            if (!await _problemRepository.AnyAsync(problemId))
            {
                throw new NotFoundException($"No existe un problema con ID {problemId}");
            }

            if (string.IsNullOrEmpty(dto.ExpectedOutput) || string.IsNullOrEmpty(dto.InputData))
            {
                throw new ValidationException("Los campos de entrada y salida esperada no pueden estar vacíos.");
            }

            TestCase test = _mapper.Map<TestCase>(dto);
            test.ProblemId = problemId;

            _testCaseRepository.Update(test);

            _ = await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TestCaseResponseDTO>(test);
        }

        // --- GET BY ID
        public async Task<TestCaseResponseDTO> GetTestCaseByIdAsync(int problemId, int id)
        {
            TestCase testCase = await _testCaseRepository.GetTestCaseByIdAsync(problemId, id) ?? throw new NotFoundException($"No existe un test case con el ID {id}.");
            return _mapper.Map<TestCaseResponseDTO>(testCase);
        }

        // --- GET LIST OF TEST CASES BY PROBLEM ID
        public async Task<List<TestCaseResponseDTO>> GetTestCasesByProblemIdAsync(int problemId, bool onlySamples = false)
        {
            IList<TestCase> testCases = await _testCaseRepository.GetTestCasesAsync(problemId, onlySamples);
            return _mapper.Map<List<TestCaseResponseDTO>>(testCases);
        }


        // --- UPDATE ---
        public async Task<TestCaseResponseDTO> UpdateTestCaseAsync(TestCaseUpdateDTO dto)
        {

            TestCase testCase = await _testCaseRepository.GetByIdAsync(dto.Id) ?? throw new NotFoundException($"No existe un problema con ID {dto.Id}");

            _ = _mapper.Map(dto, testCase);
            _ = await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TestCaseResponseDTO>(testCase);
        }

        // --- MOVE TEST TO OTHER PROBLEM ---
        public async Task<TestCaseResponseDTO> MoveTestCaseAsync(int problemId, int id, int newProblemId)
        {
            TestCase testCase = await _testCaseRepository.GetTestCaseByIdAsync(problemId, id) ?? throw new NotFoundException($"Test case {id} no encontrado.");

            testCase.ProblemId = newProblemId;

            _ = await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TestCaseResponseDTO>(testCase);
        }


        // --- DELETE ---
        public async Task DeleteTestCaseAsync(int problemId, int id)
        {
            TestCase testCase = await _testCaseRepository.GetTestCaseByIdAsync(problemId, id) ?? throw new NotFoundException($"Test case {id} no encontrado.");
            _testCaseRepository.Delete(testCase);
            _ = _unitOfWork.SaveChangesAsync();
        }

    }
}
