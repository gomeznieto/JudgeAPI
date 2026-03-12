using AutoMapper;
using JudgeAPI.Data;
using JudgeAPI.Entities;
using JudgeAPI.Excerptions;
using JudgeAPI.Models.Submission;
using Microsoft.EntityFrameworkCore;

namespace JudgeAPI.Services.Submissions
{
    public class SubmissionService : ISubmissionService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly IAnalyzer _submissionAnalyzerService;
        private readonly IConfiguration _configuration;

        public SubmissionService(
            AppDbContext appDbContext,
            IMapper mapper,
            IAnalyzer submissionAnalyzerService,
            IConfiguration configuration
        )
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _submissionAnalyzerService = submissionAnalyzerService;
            _configuration = configuration;
        }

        // --- CREATE SUBMISSION --- //
        public async Task<SubmissionResponseDTO> CreateSubmissionAsync(string userId, int problemId, SubmissionCreateDTO submissionCreateDTO)
        {
          // Verificamos el tiempo desde la última entrega
          var lastSubmission = await _appDbContext.Submissions.Where(s => s.UserId == userId).OrderByDescending(s => s.SubmissionTime).FirstOrDefaultAsync();
          var timeBetweenSubmissions = lastSubmission != null ? DateTime.UtcNow - lastSubmission.SubmissionTime : TimeSpan.Zero;

          // TIempo de espera entre submissions: 1 minuto
          var minTimeBetweenSubmissions = TimeSpan.FromMinutes(1);

          // Si el tiempo de envío es menor al lapso a esperar, se envía mensaje para que intente de vuelta más tarde
          if(timeBetweenSubmissions < minTimeBetweenSubmissions){
            throw new SubmissionTooSoonException ($"Por favor, espere {minTimeBetweenSubmissions - timeBetweenSubmissions:hh\\:mm\\:ss} antes de envíar el código.");
          }

          var submission = _mapper.Map<Submission>(submissionCreateDTO);
          submission.UserId = userId;
          submission.ProblemId = problemId;

          _appDbContext.Submissions.Add(submission);
          await _appDbContext.SaveChangesAsync();

          return _mapper.Map<SubmissionResponseDTO>(submission);
        }

        // --- GET SUBMISSION BY ID --- //
        public async Task<SubmissionResponseWrapper> GetSubmissionAsync(int id)
        {
          var result = await _appDbContext.Submissions
            .Include(s => s.Results!)
            .ThenInclude(r => r.TestCase)
            .FirstOrDefaultAsync(s => s.Id == id);

          if (result == null)
            throw new ConflictException($"No existe el resultado con el ID {id}");

          var submissionWrapper = new SubmissionResponseWrapper()
          {
            Verdict = result.Verdict,
          };

          if (submissionWrapper.IsPending)
            submissionWrapper.Summary = _mapper.Map<SubmissionResponseDTO>(result);
          else
            submissionWrapper.Results = _mapper.Map<SubmissionResponseWithResultDTO>(result);

          return submissionWrapper;
        }

        public async Task<bool> AnalyzeSubmissionAsync(int id)
        {
          return await _submissionAnalyzerService.AnalyzeAsync(id);
        }
    }
}
