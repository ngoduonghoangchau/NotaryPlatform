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
        // Flush tracked changes BEFORE committing the DB transaction. Without this,
        // the change tracker never emits its INSERT/UPDATE SQL and the transaction
        // commits an empty unit of work — a command handler that (correctly, per the
        // project rules) does not call SaveChangesAsync itself would silently persist
        // nothing. SaveChangesAsync here also triggers the EF interceptors (Outbox,
        // Auditing, SoftDelete), which only run during a save.
        await SaveChangesAsync(cancellationToken);

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
