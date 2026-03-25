using JudgeAPI.Application.Common.Interfaces;
using JudgeAPI.Domain.Entities;

namespace JudgeAPI.Application.Features.Problems.Iterfaces
{
    public interface IProblemRepository : IRepository<Problem>
    {
        Task<bool> AnyAsync(int id, CancellationToken cancellationToken = default);
    }
}
