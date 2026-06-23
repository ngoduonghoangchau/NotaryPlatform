using NotaryPlatform.Domain.Features.Core.Aggregates;
using NotaryPlatform.Domain.Features.Core.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfBranch = NotaryPlatform.Infrastructure.Persistence.Generated.Core.Branch;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Core;

public sealed class BranchRepository : RepositoryBase<EfBranch, Branch>, IBranchRepository
{
    public BranchRepository(NotaryPlatformDbContext context) : base(context) { }
}
