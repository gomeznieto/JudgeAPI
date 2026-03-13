namespace JudgeAPI.Application.Features;
using JudgeAPI.Domain;
using JudgeAPI.Application.Common.Interfaces;

public interface ISubmissionRepository : IRepository<Submission>{
    Task<List<Submission>> GetAllByUserIdAsync(string id, CancellationToken cancellationToken = default);
}
