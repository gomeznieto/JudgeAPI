using JudgeAPI.Constants;
using JudgeAPI.Data;
using JudgeAPI.Entities;
using JudgeAPI.Excerptions;
using JudgeAPI.Services.Execution;
using Microsoft.EntityFrameworkCore;

namespace JudgeAPI.Services.Submissions
{
    public class SubmissionAnalyzerService : ISubmissionAnalyzerService
    {
        private readonly ICodeCompilerService _codeCompilerService;
        private readonly ICodeExecutorService _codeExecutorService;
        private readonly AppDbContext _appDbContext;

        public SubmissionAnalyzerService(
            ICodeCompilerService codeCompilerService,
            ICodeExecutorService codeExecutorService,
            AppDbContext appDbContext
        )
        {
            _codeCompilerService = codeCompilerService;
            _codeExecutorService = codeExecutorService;
            _appDbContext = appDbContext;
        }

        public async Task<bool> AnalyzeAsync(int submissionId)
        {
            // 1. Obtener Submission y test cases
            var submission = await _appDbContext.Submissions.FindAsync(submissionId);

            if (submission is null)
                throw new NotFoundException($"Submission con ID {submissionId} no encontrada.");

            var code = submission.Code;

            if (string.IsNullOrEmpty(code))
                throw new ConflictException("No se puede analizar una submission vacía.");

            var testCases = await _appDbContext
                .TestCases.Where(t => t.ProblemId == submission.ProblemId)
                .ToListAsync();

            if(testCases.Count == 0)
                throw new ConflictException("No hay test cases asociados a este problema.");

            // 2. Llamar a _compiler.CompileAsync(...)
            var result = await _codeCompilerService.CompileAsync(code, submissionId);

            if (!result.Success)
            {
                submission.Verdict = SubmissionVerdicts.CompilationError;
                await _appDbContext.SaveChangesAsync();
                return false;
            }

            // 3. Si OK, por cada test:
            bool isAllCorrect = true;

            List<SubmissionResult> submissionResults = new List<SubmissionResult>();

            foreach (var testCase in testCases)
            {
                var executeResult = await _codeExecutorService.ExecuteAsync(submissionId, testCase, result);

                if (executeResult is null)
                    continue;

                submissionResults.Add(new SubmissionResult()
                {
                    Output = executeResult.Output,
                    ExecutionTimeMs = executeResult.ExecutionTimeMs,
                    TestCaseId = testCase.Id,
                    SubmissionId = submissionId,
                    IsCorrect = executeResult.IsCorrect
                });

                if (!executeResult.IsCorrect) isAllCorrect = false;
            }

            if (submissionResults.Count > 0)
            {
                _appDbContext.SubmissionResults.AddRange(submissionResults);
            }

            // 4. Guardamos resultados del submission
            submission.Verdict = isAllCorrect ? SubmissionVerdicts.Correct : SubmissionVerdicts.Wrong;

            // 5. Guardamos todo
            var affectedRows = await _appDbContext.SaveChangesAsync();

            return affectedRows > 0;
        }
    }
}
