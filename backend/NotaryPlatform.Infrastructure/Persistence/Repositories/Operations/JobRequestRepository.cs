using NotaryPlatform.Domain.Features.Operations.Aggregates;
using NotaryPlatform.Domain.Features.Operations.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfJobRequest = NotaryPlatform.Infrastructure.Persistence.Generated.Operations.JobRequest;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Operations;

public sealed class JobRequestRepository : RepositoryBase<EfJobRequest, JobRequest>, IJobRequestRepository
{
    public JobRequestRepository(NotaryPlatformDbContext context) : base(context) { }
}
