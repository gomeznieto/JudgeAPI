using Microsoft.EntityFrameworkCore;
using JudgeAPI.Application.Features.Submissions.Interfaces;
using JudgeAPI.Domain.Entities;
using JudgeAPI.Infrastructure.Data;

namespace JudgeAPI.Infrastructure.Persistence.Repositories.Submissions
{
    public class SubmissionRepository(AppDbContext dbContext) : ISubmissionRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        public void Add(Submission entity)
        {
            _ = _dbContext.Submissions.Add(entity);
        }

        public void Delete(Submission entity)
        {
            _ = _dbContext.Remove(entity);
        }

        public async Task<List<Submission>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Submissions.ToListAsync(cancellationToken);
        }

        public async Task<List<Submission>> GetAllByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _dbContext.Submissions.Where(s => s.UserId == userId).OrderByDescending(s => s.SubmissionTime).ToListAsync(cancellationToken);
        }

        public async Task<Submission?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Submissions.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<Submission?> GetLastSubmissionAsync(string userId, CancellationToken cancellationToken = default)
        {

            return await _dbContext.Submissions.Where(s => s.UserId == userId).OrderByDescending(s => s.SubmissionTime).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Submission?> GetSubmissionByIdAsync(int submissionId,
                                                              CancellationToken cancellationToken = default)
        {
            return await _dbContext.Submissions
                .Include(s => s.Results!)
                .ThenInclude(r => r.TestCase)
                .FirstOrDefaultAsync(s => s.Id == submissionId, cancellationToken);
        }

        public void Update(Submission entity)
        {
            _ = _dbContext.Update(entity);
        }
    }
}
