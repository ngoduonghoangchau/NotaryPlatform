using NotaryPlatform.Domain.Features.Operations.Aggregates;
using NotaryPlatform.Domain.Features.Operations.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfNotaryAvailabilityRule = NotaryPlatform.Infrastructure.Persistence.Generated.Operations.NotaryAvailabilityRule;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Operations;

public sealed class NotaryAvailabilityRuleRepository : RepositoryBase<EfNotaryAvailabilityRule, NotaryAvailabilityRule>, INotaryAvailabilityRuleRepository
{
    public NotaryAvailabilityRuleRepository(NotaryPlatformDbContext context) : base(context) { }
}
