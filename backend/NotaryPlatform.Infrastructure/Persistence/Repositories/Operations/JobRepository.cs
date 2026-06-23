using NotaryPlatform.Domain.Features.Operations.Aggregates;
using NotaryPlatform.Domain.Features.Operations.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfJob = NotaryPlatform.Infrastructure.Persistence.Generated.Operations.Job;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Operations;

public sealed class JobRepository : RepositoryBase<EfJob, Job>, IJobRepository
{
    public JobRepository(NotaryPlatformDbContext context) : base(context) { }
}
