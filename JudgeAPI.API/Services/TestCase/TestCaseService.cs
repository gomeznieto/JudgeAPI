using AutoMapper;
using JudgeAPI.Data;
using JudgeAPI.Entities;
using JudgeAPI.Excerptions;
using JudgeAPI.Models.TestCase;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace JudgeAPI.Services.TestCase
{
    public class TestCaseService : ITestCaseService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public TestCaseService(
            AppDbContext appDbContext,
            IMapper mapper
        )
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        // --- POST ---
        public async Task<TestCaseResponseDTO> CreateTestCaseAsync(int problemId, TestCaseCreateDTO create)
        {
            var problem = await _appDbContext.Problems.FindAsync(problemId);

            if (problem is null)
                throw new NotFoundException($"No existe un problema con ID {problemId}");

            if (string.IsNullOrEmpty(create.ExpectedOutput) || string.IsNullOrEmpty(create.InputData))
                throw new ValidationException("Los campos de entrada y salida esperada no pueden estar vacíos.");

            var test = _mapper.Map<Entities.TestCase>(create);
            test.ProblemId = problemId;

            _appDbContext.Add(test);
            await _appDbContext.SaveChangesAsync();

            return _mapper.Map<TestCaseResponseDTO>(test);
        }

        // --- GET BY ID
        public async Task<TestCaseResponseDTO> GetTestCaseByIdAsync(int problemId, int id)
        {
            var problem = await _appDbContext.Problems.FindAsync(problemId);

            if (problem is null)
                throw new NotFoundException($"No existe un problema con ID {problemId}");

            var testCase = await _appDbContext.TestCases.FindAsync(id);

            if (testCase is null)
                throw new NotFoundException($"No existe un test case con el ID {id}.");

            return _mapper.Map<TestCaseResponseDTO>(testCase);
        }

        // --- GET LIST OF TEST CASES BY PROBLEM ID
        public async Task<List<TestCaseResponseDTO>> GetTestCasesByProblemIdAsync(int problemId, bool onlySamples = false)
        {
            var problem = await _appDbContext.Problems.FindAsync(problemId);

            if (problem is null)
                throw new NotFoundException($"No existe un problema con ID {problemId}");

            var query = _appDbContext.TestCases.Where(t => t.ProblemId == problemId);

            if (onlySamples)
                query = query.Where(t => t.IsSample);

            var testCases = await query.ToListAsync();

            return _mapper.Map<List<TestCaseResponseDTO>>(query);
        }


        // --- UPDATE ---
        public async Task<TestCaseResponseDTO> UpdateTestCaseAsync(TestCaseUpdateDTO update)
        {
            var testCase = await _appDbContext.TestCases.FindAsync(update.Id);

            if (testCase is null)
                throw new NotFoundException($"Test case {update.Id} no encontrado.");

            _mapper.Map(update, testCase);

            await _appDbContext.SaveChangesAsync();

            return _mapper.Map<TestCaseResponseDTO>(testCase);
        }

        // --- MOVE TEST TO OTHER PROBLEM ---
        public async Task<TestCaseResponseDTO> MoveTestCaseAsync(int problemId, int id, int newProblemId)
        {
            var problem = await _appDbContext.Problems.FindAsync(problemId);

            if (problem is null)
                throw new NotFoundException($"No existe un problema con ID {problemId}");

            var testCase = await _appDbContext.TestCases.FindAsync(id);

            if (testCase is null)
                throw new NotFoundException($"Test case {id} no encontrado.");

            testCase.ProblemId = newProblemId;

            await _appDbContext.SaveChangesAsync();

            return _mapper.Map<TestCaseResponseDTO>(testCase);

        }


        // --- DELETE ---
        public async Task DeleteTestCaseAsync(int id)
        {
            var exists = await _appDbContext.TestCases.AnyAsync(t => t.Id == id);

            if (!exists)
                throw new NotFoundException($"Test case {id} no econtrado.");

            var affectedRows = await _appDbContext.TestCases.Where(t => t.Id == id).ExecuteDeleteAsync();

            if(affectedRows == 0)
                throw new InvalidOperationException($"No se pudo eliminar el test case con ID {id}.");
        }

    }
}
