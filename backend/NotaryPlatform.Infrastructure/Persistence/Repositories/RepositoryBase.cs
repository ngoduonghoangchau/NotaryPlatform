using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NotaryPlatform.Domain.Common;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories;

public abstract class RepositoryBase<TEfEntity, TDomainEntity>
    : IRepositoryBase<TDomainEntity>
    where TEfEntity : class
    where TDomainEntity : class
{
    protected readonly NotaryPlatformDbContext Context;

    protected DbSet<TEfEntity> DbSet => Context.Set<TEfEntity>();

    protected RepositoryBase(NotaryPlatformDbContext context)
        => Context = context;

    // ── IRepositoryBase<TDomainEntity> ──────────────────────────────────────

    public virtual Task<TDomainEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => throw new NotImplementedException($"{GetType().Name}.GetByIdAsync: add Reconstitute(…) to the domain aggregate, then override this method.");

    public virtual Task AddAsync(TDomainEntity entity, CancellationToken cancellationToken = default)
        => throw new NotImplementedException($"{GetType().Name}.AddAsync: implement FromDomain mapping, then override this method.");

    public virtual Task UpdateAsync(TDomainEntity entity, CancellationToken cancellationToken = default)
        => throw new NotImplementedException($"{GetType().Name}.UpdateAsync: implement FromDomain mapping, then override this method.");

    public virtual Task DeleteAsync(TDomainEntity entity, CancellationToken cancellationToken = default)
        => throw new NotImplementedException($"{GetType().Name}.DeleteAsync: implement FromDomain mapping, then override this method.");

    // ── EF read helpers ──────────────────────────────────────────────────────

    protected Task<TEfEntity?> FindOneAsync(
        Expression<Func<TEfEntity, bool>> predicate,
        CancellationToken ct)
        => DbSet.AsNoTracking().FirstOrDefaultAsync(predicate, ct);

    protected async Task<IReadOnlyList<TEfEntity>> FindManyAsync(
        Expression<Func<TEfEntity, bool>> predicate,
        CancellationToken ct)
    {
        var list = await DbSet.AsNoTracking().Where(predicate).ToListAsync(ct);
        return list.AsReadOnly();
    }

    // ── EF write helpers ─────────────────────────────────────────────────────

    protected ValueTask<EntityEntry<TEfEntity>>
        AddEntityAsync(TEfEntity entity, CancellationToken ct)
        => DbSet.AddAsync(entity, ct);

    protected void UpdateEntity(TEfEntity entity)
        => DbSet.Update(entity);

    protected async Task RemoveByPkAsync(Guid pkValue, CancellationToken ct)
    {
        var entity = await DbSet.FindAsync([pkValue], ct);
        if (entity is not null)
            DbSet.Remove(entity);
    }
}
