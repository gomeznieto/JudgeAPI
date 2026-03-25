using JudgeAPI.Application.Features.TestCases.Interfaces;
using JudgeAPI.Domain.Entities;
using JudgeAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JudgeAPI.Infrastructure.Persistence.Repositories.TestCases
{

    public class TestCaseRepository(AppDbContext dbContext) : ITestCaseRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        public void Add(TestCase entity)
        {
            _ = _dbContext.TestCases.Add(entity);
        }

        public Task<bool> AnyAsync(int id)
        {
            return _dbContext.TestCases.AnyAsync(t => t.Id == id);
        }

        public void Delete(TestCase entity)
        {
            _ = _dbContext.TestCases.Remove(entity);
        }

        public async Task<List<TestCase>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _dbContext.TestCases.ToListAsync(cancellationToken);
        }

        public async Task<TestCase?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.TestCases.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<TestCase?> GetTestCaseByIdAsync(int problemId, int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.TestCases.FirstOrDefaultAsync(t => t.Id == id && t.ProblemId == problemId, cancellationToken);
        }

        public async Task<IList<TestCase>> GetTestCasesAsync(int problemId, bool onlySamples = false, CancellationToken cancellationToken = default)
        {
            return await _dbContext.TestCases.Where(t => t.ProblemId == problemId && (!onlySamples || t.IsSample)).ToListAsync(cancellationToken);
        }

        public void Update(TestCase entity)
        {
            _ = _dbContext.TestCases.Update(entity);
        }
    }
}
