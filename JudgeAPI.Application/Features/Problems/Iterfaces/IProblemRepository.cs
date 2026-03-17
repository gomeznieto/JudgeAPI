using JudgeAPI.Application.Common.Interfaces;
using JudgeAPI.Domain;

namespace JudgeAPI.Application.Features
{
    public interface IProblemRepository : IRepository<Problem>{

        Task<bool> AnyAsync(int id, CancellationToken cancellationToken = default);
    }
}
