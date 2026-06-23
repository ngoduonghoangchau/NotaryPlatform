using NotaryPlatform.Domain.Features.Compliance.Aggregates;
using NotaryPlatform.Domain.Features.Compliance.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfRetentionJob = NotaryPlatform.Infrastructure.Persistence.Generated.Compliance.RetentionJob;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Compliance;

public sealed class RetentionJobRepository : RepositoryBase<EfRetentionJob, RetentionJob>, IRetentionJobRepository
{
    public RetentionJobRepository(NotaryPlatformDbContext context) : base(context) { }
}
