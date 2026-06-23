using NotaryPlatform.Domain.Features.Compliance.Aggregates;
using NotaryPlatform.Domain.Features.Compliance.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfRetentionPolicy = NotaryPlatform.Infrastructure.Persistence.Generated.Compliance.RetentionPolicy;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Compliance;

public sealed class RetentionPolicyRepository : RepositoryBase<EfRetentionPolicy, RetentionPolicy>, IRetentionPolicyRepository
{
    public RetentionPolicyRepository(NotaryPlatformDbContext context) : base(context) { }
}
