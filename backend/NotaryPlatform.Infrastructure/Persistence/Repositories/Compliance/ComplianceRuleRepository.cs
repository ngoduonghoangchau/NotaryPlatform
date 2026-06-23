using NotaryPlatform.Domain.Features.Compliance.Aggregates;
using NotaryPlatform.Domain.Features.Compliance.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfComplianceRule = NotaryPlatform.Infrastructure.Persistence.Generated.Compliance.ComplianceRule;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Compliance;

public sealed class ComplianceRuleRepository : RepositoryBase<EfComplianceRule, ComplianceRule>, IComplianceRuleRepository
{
    public ComplianceRuleRepository(NotaryPlatformDbContext context) : base(context) { }
}
