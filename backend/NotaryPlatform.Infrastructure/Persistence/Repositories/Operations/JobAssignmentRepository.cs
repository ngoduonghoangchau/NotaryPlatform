using NotaryPlatform.Domain.Features.Operations.Aggregates;
using NotaryPlatform.Domain.Features.Operations.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfJobAssignment = NotaryPlatform.Infrastructure.Persistence.Generated.Operations.JobAssignment;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Operations;

public sealed class JobAssignmentRepository : RepositoryBase<EfJobAssignment, JobAssignment>, IJobAssignmentRepository
{
    public JobAssignmentRepository(NotaryPlatformDbContext context) : base(context) { }
}
