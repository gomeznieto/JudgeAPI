using JudgeAPI.Application.Common;
using JudgeAPI.Application.Common.Interfaces;
using JudgeAPI.Application.Features.CodeExecutor.Interfaces;
using JudgeAPI.Application.Features.SubmissionResults.Interfaces;
using JudgeAPI.Application.Features.Submissions.Interfaces;
using JudgeAPI.Application.Features.TestCases.Interfaces;
using JudgeAPI.Domain;
using JudgeAPI.Domain.Entities;

namespace JudgeAPI.Application.Features.CodeExecutor.Services
{
    public class LocalAnalyzer(
        ICodeCompilerService codeCompilerService,
        ICodeExecutorService codeExecutorService,
        ISubmissionRepository submissionRepository,
        ITestCaseRepository testCaseRepository,
        ISubmissionResultsRepository submissionResultRepository,
        IUnitOfWork unitOfWork
        ) : IAnalyzer
    {
        private readonly ICodeCompilerService _codeCompilerService = codeCompilerService;
        private readonly ICodeExecutorService _codeExecutorService = codeExecutorService;
        private readonly ISubmissionRepository _submissionRepository = submissionRepository;
        private readonly ITestCaseRepository _testCaseRepository = testCaseRepository;
        private readonly ISubmissionResultsRepository _submissionResultRepository = submissionResultRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        // Revisión de código local
        public async Task<bool> AnalyzeAsync(int submissionId)
        {
            // 1. Obtener Submission y test cases
            Submission submission = await _submissionRepository.GetByIdAsync(submissionId) ?? throw new NotFoundException($"Submission con ID {submissionId} no encontrada.");

            string? code = submission.Code;

            if (string.IsNullOrEmpty(code))
            {
                throw new ConflictException("No se puede analizar una submission vacía.");
            }

            IList<TestCase> testCases = await _testCaseRepository.GetTestCasesAsync(submission.ProblemId);

            if (testCases.Count == 0)
            {
                throw new ConflictException("No hay test cases asociados a este problema.");
            }

            // 2. Llamar a _compiler.CompileAsync(...)
            var result = await _codeCompilerService.CompileAsync(code, submissionId);

            if (!result.Success)
            {
                submission.Verdict = SubmissionVerdicts.CompilationError;
                _ = await _unitOfWork.SaveChangesAsync();
                return false;
            }

            // 3. Si OK, por cada test:
            bool isAllCorrect = true;

            List<SubmissionResult> submissionResults = [];

            foreach (TestCase testCase in testCases)
            {
                var executeResult = await _codeExecutorService.ExecuteAsync(submissionId, testCase, result);

                if (executeResult is null)
                {
                    continue;
                }

                submissionResults.Add(new SubmissionResult()
                {
                    Output = executeResult.Output,
                    ExecutionTimeMs = executeResult.ExecutionTimeMs,
                    TestCaseId = testCase.Id,
                    SubmissionId = submissionId,
                    IsCorrect = executeResult.IsCorrect
                });

                if (!executeResult.IsCorrect)
                {
                    isAllCorrect = false;
                }
            }

            if (submissionResults.Count > 0)
            {
                _submissionResultRepository.AddRange(submissionResults);
            }

            // 4. Guardamos resultados del submission
            submission.Verdict = isAllCorrect ? SubmissionVerdicts.Correct : SubmissionVerdicts.Wrong;

            // 5. Guardamos todo
            int affectedRows = await _unitOfWork.SaveChangesAsync();

            return affectedRows > 0;
        }
    }
}
