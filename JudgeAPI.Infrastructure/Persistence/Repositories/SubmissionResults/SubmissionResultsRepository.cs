using JudgeAPI.Application.Features.SubmissionResults.Interfaces;
using JudgeAPI.Domain.Entities;
using JudgeAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JudgeAPI.Infrastructure.Persistence.Repositories.SubmissionResults
{
    public class SubmissionResultRepository(AppDbContext appDbContext) : ISubmissionResultsRepository
    {
        private readonly AppDbContext _dbContext = appDbContext;

        public void Add(SubmissionResult entity)
        {
            _ = _dbContext.SubmissionResults.Add(entity);
        }

        public void AddRange(IList<SubmissionResult> submissionResults)
        {
            _dbContext.AddRange(submissionResults);
        }

        public void Delete(SubmissionResult entity)
        {
            _ = _dbContext.SubmissionResults.Remove(entity);
        }

        public async Task<List<SubmissionResult>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SubmissionResults.ToListAsync(cancellationToken);
        }

        public async Task<SubmissionResult?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.SubmissionResults.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public void Update(SubmissionResult entity)
        {
            _ = _dbContext.SubmissionResults.Update(entity);
        }
    }
}
