using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace NotaryPlatform.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Converts hard deletes into soft deletes for any entity that exposes a
/// nullable <c>DeletedAt</c> property. The entity's state is changed from
/// <see cref="EntityState.Deleted"/> to <see cref="EntityState.Modified"/>
/// and <c>DeletedAt</c> (plus <c>UpdatedAt</c> when present) is stamped with
/// the current UTC time.
/// </summary>
public sealed class SoftDeleteSaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        ApplySoftDelete(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplySoftDelete(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void ApplySoftDelete(DbContext? context)
    {
        if (context is null) return;

        foreach (var entry in context.ChangeTracker
            .Entries()
            .Where(e => e.State == EntityState.Deleted)
            .ToList())
        {
            var deletedAtProp = FindTimestampProperty(entry, "DeletedAt");
            if (deletedAtProp is null) continue;

            // Revert to Modified so EF issues an UPDATE instead of DELETE
            entry.State = EntityState.Modified;

            StampNow(deletedAtProp);
            StampNow(FindTimestampProperty(entry, "UpdatedAt"));
        }
    }

    private static PropertyEntry? FindTimestampProperty(EntityEntry entry, string name)
        => entry.Properties.FirstOrDefault(p => p.Metadata.Name == name);

    private static void StampNow(PropertyEntry? prop)
    {
        if (prop is null) return;

        var clrType = prop.Metadata.ClrType;

        if (clrType == typeof(DateTime) || clrType == typeof(DateTime?))
            prop.CurrentValue = DateTime.UtcNow;
        else if (clrType == typeof(DateTimeOffset) || clrType == typeof(DateTimeOffset?))
            prop.CurrentValue = DateTimeOffset.UtcNow;
    }
}
