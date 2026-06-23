using NotaryPlatform.Domain.Features.Core.Aggregates;
using NotaryPlatform.Domain.Features.Core.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfTenant = NotaryPlatform.Infrastructure.Persistence.Generated.Core.Tenant;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Core;

public sealed class TenantRepository : RepositoryBase<EfTenant, Tenant>, ITenantRepository
{
    public TenantRepository(NotaryPlatformDbContext context) : base(context) { }
}
