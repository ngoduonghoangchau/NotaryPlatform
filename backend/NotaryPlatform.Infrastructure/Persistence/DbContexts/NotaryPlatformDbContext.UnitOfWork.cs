using Microsoft.EntityFrameworkCore.Storage;
using NotaryPlatform.Application.Abstractions.Persistence;

namespace NotaryPlatform.Infrastructure.Persistence.DbContexts;

public partial class NotaryPlatformDbContext : IUnitOfWork
{
    private IDbContextTransaction? _currentTransaction;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _currentTransaction = await Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _currentTransaction!.CommitAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _currentTransaction!.RollbackAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }
}
