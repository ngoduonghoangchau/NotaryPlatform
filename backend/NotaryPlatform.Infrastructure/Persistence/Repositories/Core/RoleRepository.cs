using NotaryPlatform.Domain.Features.Core.Aggregates;
using NotaryPlatform.Domain.Features.Core.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfRole = NotaryPlatform.Infrastructure.Persistence.Generated.Core.Role;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Core;

public sealed class RoleRepository : RepositoryBase<EfRole, Role>, IRoleRepository
{
    public RoleRepository(NotaryPlatformDbContext context) : base(context) { }
}
