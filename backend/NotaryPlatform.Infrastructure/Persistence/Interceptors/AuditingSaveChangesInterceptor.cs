using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Shared.Interfaces;

namespace NotaryPlatform.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Automatically stamps <c>CreatedAt</c> on new entities and <c>UpdatedAt</c>
/// on every changed entity before EF Core executes SQL.
/// <para>
/// Although the database also has <c>set_updated_at()</c> triggers, this
/// interceptor keeps the in-memory entity consistent so callers see the
/// correct timestamp immediately after <c>SaveChanges</c> returns.
/// </para>
/// <para>
/// <see cref="ICurrentUser"/> is resolved per-call via
/// <see cref="IServiceScopeFactory"/> because it is a scoped service while
/// this interceptor is registered as a singleton.
/// </para>
/// </summary>
public sealed class AuditingSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IServiceScopeFactory _scopeFactory;

    public AuditingSaveChangesInterceptor(IServiceScopeFactory scopeFactory)
        => _scopeFactory = scopeFactory;

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        StampTimestamps(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        StampTimestamps(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void StampTimestamps(DbContext? context)
    {
        if (context is null) return;

        // Resolve ICurrentUser per-call (it is a scoped service)
        using var scope = _scopeFactory.CreateScope();
        _ = scope.ServiceProvider.GetService<ICurrentUser>();

        var now = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker
            .Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified))
        {
            if (entry.State == EntityState.Added)
                TrySet(entry, "CreatedAt", now);

            TrySet(entry, "UpdatedAt", now);
        }
    }

    private static void TrySet(EntityEntry entry, string propertyName, DateTime utcNow)
    {
        var prop = entry.Properties
            .FirstOrDefault(p => p.Metadata.Name == propertyName);

        if (prop is null || prop.IsTemporary) return;

        // Respect values already set explicitly by the caller
        if (entry.State == EntityState.Added && prop.CurrentValue is not null
            && prop.CurrentValue is DateTime dt && dt != default)
            return;

        var clrType = prop.Metadata.ClrType;

        if (clrType == typeof(DateTime) || clrType == typeof(DateTime?))
            prop.CurrentValue = utcNow;
        else if (clrType == typeof(DateTimeOffset) || clrType == typeof(DateTimeOffset?))
            prop.CurrentValue = new DateTimeOffset(utcNow, TimeSpan.Zero);
    }
}
