using JudgeAPI.Domain;
using JudgeAPI.Application.Common.Interfaces;

namespace JudgeAPI.Application.Features.Submissions.Interfaces
{
    public interface ISubmissionRepository : IRepository<Submission>
    {
        Task<List<Submission>> GetAllByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<Submission?>GetLastSubmissionAsync(string userId, CancellationToken cancellationToken = default);
        Task<Submission?> GetSubmissionByIdAsync(int submissionId, CancellationToken cancellationToken = default);
    }
}
