using AutoMapper;
using JudgeAPI.Application.Common.Interfaces;
using JudgeAPI.Application.Features.Submissions.Dtos;
using JudgeAPI.Application.Features.Submissions.Interfaces;
using JudgeAPI.Application.Features.CodeExecutor.Interfaces;
using JudgeAPI.Domain.Entities;
using JudgeAPI.Application.Common.Exceptions;

namespace JudgeAPI.Application.Features.Submissions.Services
{
    public class SubmissionService(
            IMapper mapper,
            IAnalyzer submissionAnalyzerService,
            ISubmissionRepository submissionRepository,
            IUnitOfWork unitOfWork
            )
        : ISubmissionService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IAnalyzer _submissionAnalyzerService = submissionAnalyzerService;
        private readonly ISubmissionRepository _submissionRespository = submissionRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        // --- CREATE SUBMISSION --- //
        public async Task<SubmissionResponseDTO> CreateSubmissionAsync(string userId, int problemId, SubmissionCreateDTO submissionCreateDTO)
        {
            Submission? lastSubmission = await _submissionRespository.GetLastSubmissionAsync(userId);

            if (lastSubmission != null)
            {
                TimeSpan timeBetweenSubmissions = DateTime.UtcNow - lastSubmission.SubmissionTime;
                TimeSpan minTimeBetweenSubmissions = TimeSpan.FromMinutes(1);

                if (timeBetweenSubmissions < minTimeBetweenSubmissions)
                {
                    throw new SubmissionTooSoonException($"Por favor, espere {minTimeBetweenSubmissions - timeBetweenSubmissions:hh\\:mm\\:ss} antes de envíar el código.");
                }
            }


            Submission submission = _mapper.Map<Submission>(submissionCreateDTO);
            submission.UserId = userId;
            submission.ProblemId = problemId;

            _submissionRespository.Add(submission);
            _ = await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<SubmissionResponseDTO>(submission);
        }

        // --- GET SUBMISSION BY ID --- //
        public async Task<SubmissionResponseDTO> GetSubmissionAsync(int submissionId)
        {
            Submission? result = await _submissionRespository.GetSubmissionByIdAsync(submissionId) ?? throw new KeyNotFoundException($"No existe el resultado con el ID {submissionId}");


            SubmissionResponseDTO submissionResponse = _mapper.Map<SubmissionResponseDTO>(result);
            submissionResponse.Verdict = result.Verdict;

            return submissionResponse;
        }

        public async Task<bool> AnalyzeSubmissionAsync(int submissionId)
        {
            return await _submissionAnalyzerService.AnalyzeAsync(submissionId);
        }
    }
}
