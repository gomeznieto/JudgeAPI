using JudgeAPI.Application.Common.Interfaces;
using JudgeAPI.Domain.Entities;

namespace JudgeAPI.Application.Features.Units.Interfaces
{
    public interface IUnitRepository : IRepository<Unit>
    {
        Task<Unit?> GetUnitWithProblemByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
