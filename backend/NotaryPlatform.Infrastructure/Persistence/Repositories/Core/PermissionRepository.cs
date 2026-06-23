using NotaryPlatform.Domain.Features.Core.Aggregates;
using NotaryPlatform.Domain.Features.Core.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfPermission = NotaryPlatform.Infrastructure.Persistence.Generated.Core.Permission;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Core;

public sealed class PermissionRepository : RepositoryBase<EfPermission, Permission>, IPermissionRepository
{
    public PermissionRepository(NotaryPlatformDbContext context) : base(context) { }
}
