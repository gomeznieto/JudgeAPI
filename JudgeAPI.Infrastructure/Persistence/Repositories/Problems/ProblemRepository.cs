
using JudgeAPI.Application.Features;
using JudgeAPI.Domain;
using Microsoft.EntityFrameworkCore;

namespace JudgeAPI.Infrastructure.Persistence.Repositories.Problems
{
    public class ProblemRepository(AppDbContext dbContext) : IProblemRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        public void Add(Problem entity)
        {
           _ = _dbContext.Problems.Add(entity);
        }

        public void Delete(Problem entity)
        {
            _ = _dbContext.Problems.Remove(entity);
        }

        public async Task<List<Problem>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Problems.ToListAsync(cancellationToken);
        }

        public async Task<Problem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Problems.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public void Update(Problem entity)
        {
            _ = _dbContext.Problems.Update(entity);
        }

        public Task<bool> AnyAsync(int id, CancellationToken cancellationToken = default){
            return _dbContext.Problems.AnyAsync(p => p.Id == id, cancellationToken);
        }
    }
}
