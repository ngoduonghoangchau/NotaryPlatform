using NotaryPlatform.Domain.Features.Operations.Aggregates;
using NotaryPlatform.Domain.Features.Operations.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfDispatchRule = NotaryPlatform.Infrastructure.Persistence.Generated.Operations.DispatchRule;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Operations;

public sealed class DispatchRuleRepository : RepositoryBase<EfDispatchRule, DispatchRule>, IDispatchRuleRepository
{
    public DispatchRuleRepository(NotaryPlatformDbContext context) : base(context) { }
}
