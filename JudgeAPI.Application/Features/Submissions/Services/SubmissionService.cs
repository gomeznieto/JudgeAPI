using AutoMapper;
using JudgeAPI.Domain;
using JudgeAPI.Application.Common.Interfaces;
using JudgeAPI.Application.Common;
using JudgeAPI.Application.Features;

namespace JudgeAPI.Services.Submissions
{
    public class SubmissionService : ISubmissionService
    {
        private readonly IMapper _mapper;
        private readonly IAnalyzer _submissionAnalyzerService;
        private readonly ISubmissionRepository _submissionRespository;
        private readonly IUnitOfWork _unitOfWork;

        public SubmissionService(
            IMapper mapper,
            IAnalyzer submissionAnalyzerService,
            ISubmissionRepository submissionRepository,
            IUnitOfWork unitOfWork
        )
        {
            _mapper = mapper;
            _submissionAnalyzerService = submissionAnalyzerService;
            _submissionRespository = submissionRepository;
            _unitOfWork = unitOfWork;
        }

        // --- CREATE SUBMISSION --- //
        public async Task<SubmissionResponseDTO> CreateSubmissionAsync(string userId, int problemId, SubmissionCreateDTO submissionCreateDTO)
        {
          var lastSubmission = await _submissionRespository.GetLastSubmissionAsync(userId);
          var timeBetweenSubmissions = lastSubmission != null ? DateTime.UtcNow - lastSubmission.SubmissionTime : TimeSpan.Zero;
          var minTimeBetweenSubmissions = TimeSpan.FromMinutes(1);

          if(timeBetweenSubmissions < minTimeBetweenSubmissions){
            throw new SubmissionTooSoonException ($"Por favor, espere {minTimeBetweenSubmissions - timeBetweenSubmissions:hh\\:mm\\:ss} antes de envíar el código.");
          }

          var submission = _mapper.Map<Submission>(submissionCreateDTO);
          submission.UserId = userId;
          submission.ProblemId = problemId;

          _submissionRespository.Add(submission);
          await _unitOfWork.SaveChangesAsync();

          return _mapper.Map<SubmissionResponseDTO>(submission);
        }

        // --- GET SUBMISSION BY ID --- //
        public async Task<SubmissionResponseDTO> GetSubmissionAsync(int submissionId)
        {
            var result = await _submissionRespository.GetSubmissionByIdAsync(submissionId); 

            if (result == null)
                throw new KeyNotFoundException($"No existe el resultado con el ID {submissionId}");

            var submissionResponse = _mapper.Map<SubmissionResponseDTO>(result);
            submissionResponse.Verdict = result.Verdict;

            return submissionResponse;
        }

        public async Task<bool> AnalyzeSubmissionAsync(int id)
        {
            return await _submissionAnalyzerService.AnalyzeAsync(id);
        }
    }
}
