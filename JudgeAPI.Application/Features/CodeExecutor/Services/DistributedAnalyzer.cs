
using System.Text.Json;
using JudgeAPI.Application.Common.Exceptions;
using JudgeAPI.Application.Common.Interfaces;
using JudgeAPI.Application.Features.CodeExecutor.Interfaces;
using JudgeAPI.Application.Features.Submissions.Interfaces;
using JudgeAPI.Domain.Constants;
using JudgeAPI.Domain.Entities;

namespace JudgeAPI.Application.Features.CodeExecutor.Services
{
    public class DistributedAnalyzer(
            ISubmissionRepository submissionRepository,
            IUnitOfWork unitOfWork,
            ICacheService cacheService
                ) : IAnalyzer
    {
        private readonly ISubmissionRepository _submissionRepository = submissionRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICacheService _cacheService = cacheService;

        // --- ARMA EL SUBMISSION PARA QUE LO ANALICE EL RUNNER ---//
        public async Task<bool> AnalyzeAsync(int submissionId)
        {
            Submission submission = await _submissionRepository.GetByIdAsync(submissionId) ?? throw new NotFoundException($"Submission con ID {submissionId} no encontrada.");

            submission.Verdict = SubmissionVerdicts.Queued;
            _ = await _unitOfWork.SaveChangesAsync();

            // Colocamos el submission en Redis
            string job = JsonSerializer.Serialize(new { SubmissionId = submissionId });
            await _cacheService.ListRightPushAsync("submissions", job);

            return true; // TODO: ¿Es Necesario que devuelva un bool?
        }
    }
}
