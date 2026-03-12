
using JudgeAPI.Data;
using StackExchange.Redis;
using JudgeAPI.Excerptions;
using JudgeAPI.Constants;
using System.Text.Json;

namespace JudgeAPI.Services.Submissions
{
    public class DistributedAnalyzer : IAnalyzer
    {
        private readonly AppDbContext _appDbContext;
        private readonly IConnectionMultiplexer _redis;

        public DistributedAnalyzer(
            AppDbContext appDbContext,
            IConnectionMultiplexer redis
        )
        {
            _appDbContext = appDbContext;
            _redis = redis;
        }

        public async Task<bool> AnalyzeAsync(int submissionId)
        {
            var submission = await _appDbContext.Submissions.FindAsync(submissionId);

            if (submission is null)
                throw new NotFoundException($"Submission con ID {submissionId} no encontrada.");

            submission.Verdict = SubmissionVerdicts.Queued;
            await _appDbContext.SaveChangesAsync();

            var job = JsonSerializer.Serialize(new { SubmissionId = submissionId});
            await _redis.GetDatabase().ListRightPushAsync("submissions", job);

            return true;
        }
    }
}
