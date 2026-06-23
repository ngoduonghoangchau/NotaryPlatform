using NotaryPlatform.Domain.Features.Compliance.Aggregates;
using NotaryPlatform.Domain.Features.Compliance.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfPolicyVersion = NotaryPlatform.Infrastructure.Persistence.Generated.Compliance.PolicyVersion;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Compliance;

public sealed class PolicyVersionRepository : RepositoryBase<EfPolicyVersion, PolicyVersion>, IPolicyVersionRepository
{
    public PolicyVersionRepository(NotaryPlatformDbContext context) : base(context) { }
}
