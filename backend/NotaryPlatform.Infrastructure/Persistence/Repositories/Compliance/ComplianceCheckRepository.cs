using NotaryPlatform.Domain.Features.Compliance.Aggregates;
using NotaryPlatform.Domain.Features.Compliance.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfComplianceCheck = NotaryPlatform.Infrastructure.Persistence.Generated.Compliance.ComplianceCheck;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Compliance;

public sealed class ComplianceCheckRepository : RepositoryBase<EfComplianceCheck, ComplianceCheck>, IComplianceCheckRepository
{
    public ComplianceCheckRepository(NotaryPlatformDbContext context) : base(context) { }
}
