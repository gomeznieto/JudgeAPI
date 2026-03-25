using JudgeAPI.Application.Common.Interfaces;
using JudgeAPI.Domain.Entities;

namespace JudgeAPI.Application.Features.SubmissionResults.Interfaces
{
    public interface ISubmissionResultsRepository : IRepository<SubmissionResult>
    {
        void AddRange(IList<SubmissionResult> submissionResults);
    }
}
