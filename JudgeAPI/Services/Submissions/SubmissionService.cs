using AutoMapper;
using JudgeAPI.Data;
using JudgeAPI.Entities;
using JudgeAPI.Excerptions;
using JudgeAPI.Models.Submission;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

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

        public async Task<SubmissionResponseDTO> CreateSubmissionAsync(string userId, int problemId, SubmissionCreateDTO submissionCreateDTO)
        {
            var submission = _mapper.Map<Submission>(submissionCreateDTO);
            submission.UserId = userId;
            submission.ProblemId = problemId;

            _appDbContext.Submissions.Add(submission);
            await _appDbContext.SaveChangesAsync();

            return _mapper.Map<SubmissionResponseDTO>(submission);
        }

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
