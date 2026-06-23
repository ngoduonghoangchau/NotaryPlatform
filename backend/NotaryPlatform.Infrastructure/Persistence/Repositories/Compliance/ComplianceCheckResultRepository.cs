using NotaryPlatform.Domain.Features.Compliance.Aggregates;
using NotaryPlatform.Domain.Features.Compliance.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfComplianceCheckResult = NotaryPlatform.Infrastructure.Persistence.Generated.Compliance.ComplianceCheckResult;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Compliance;

public sealed class ComplianceCheckResultRepository : RepositoryBase<EfComplianceCheckResult, ComplianceCheckResult>, IComplianceCheckResultRepository
{
    public ComplianceCheckResultRepository(NotaryPlatformDbContext context) : base(context) { }
}
