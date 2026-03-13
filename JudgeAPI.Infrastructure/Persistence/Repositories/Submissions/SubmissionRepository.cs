namespace JudgeAPI.Infrastructure;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JudgeAPI.Application.Features;
using JudgeAPI.Domain;
using Microsoft.EntityFrameworkCore;

public class SubmissionRepository : ISubmissionRepository
{
    private readonly AppDbContext _dbContext;

    public SubmissionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public void Add(Submission entity)
    {
        _dbContext.Submissions.Add(entity);
    }

    public void Delete(Submission entity)
    {
        _dbContext.Remove(entity);
    }

    public async Task<List<Submission>> GetAll(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Submissions.ToListAsync();
    }

    public async Task<List<Submission>> GetAllByUserIdAsync(string id, CancellationToken cancellationToken)
    {
        return await _dbContext.Submissions.Where(s => s.UserId == id).ToListAsync();
    }

    public async Task<Submission?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Submissions.FirstOrDefaultAsync(s => s.Id == id);
    }

    public void Update(Submission entity)
    {
        _dbContext.Update(entity);
    }
}
