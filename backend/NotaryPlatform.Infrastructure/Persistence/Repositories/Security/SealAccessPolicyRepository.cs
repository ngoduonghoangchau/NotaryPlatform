using NotaryPlatform.Domain.Features.Security.Aggregates;
using NotaryPlatform.Domain.Features.Security.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfSealAccessPolicy = NotaryPlatform.Infrastructure.Persistence.Generated.Security.SealAccessPolicy;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Security;

public sealed class SealAccessPolicyRepository : RepositoryBase<EfSealAccessPolicy, SealAccessPolicy>, ISealAccessPolicyRepository
{
    public SealAccessPolicyRepository(NotaryPlatformDbContext context) : base(context) { }
}
