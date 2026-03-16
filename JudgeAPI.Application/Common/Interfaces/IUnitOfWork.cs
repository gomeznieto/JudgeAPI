namespace JudgeAPI.Application.Common.Interfaces;

public interface IUnitOfWork : IDisposable {
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
