namespace NotaryPlatform.Application.Common.Interfaces;

/// <summary>
/// Abstracts EF Core's DbContext transaction and save semantics so that
/// the Application layer stays independent of any persistence technology.
/// Implemented by Infrastructure.Persistence.DbContexts.NotaryPlatformDbContext.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
