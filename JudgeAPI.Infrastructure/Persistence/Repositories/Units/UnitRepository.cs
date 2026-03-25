using JudgeAPI.Application.Features.Units.Interfaces;
using JudgeAPI.Domain.Entities;
using JudgeAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class UnitRespository(AppDbContext dbContext) : IUnitRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public void Add(Unit entity)
    {
        _ = _dbContext.Units.Add(entity);
    }

    public void Delete(Unit entity)
    {
        _ = _dbContext.Units.Remove(entity);
    }

    public async Task<List<Unit>> GetAll(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Units.Where(static u => u.IsActivate).ToListAsync(cancellationToken);
    }

    public async Task<Unit?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Units.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<Unit?> GetUnitWithProblemByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.Units
            .Include(u => u.Problems)
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public void Update(Unit entity)
    {
        _ = _dbContext.Units.Update(entity);
    }
}
