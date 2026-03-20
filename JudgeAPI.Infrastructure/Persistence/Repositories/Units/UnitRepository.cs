using JudgeAPI.Application.Features.Units.Interfaces;
using JudgeAPI.Domain;
using JudgeAPI.Infrastructure;

public class UnitRespository(AppDbContext dbContext) : IUnitRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public void Add(Unit entity)
    {
        _dbContext.Units.Add(entity);
    }

    public void Delete(Unit entity)
    {
        _dbContext.Units.Remove(entity);
    }

    public Task<List<Unit>> GetAll(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Unit?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Update(Unit entity)
    {
        _dbContext.Units.Update(entity);
    }
}
