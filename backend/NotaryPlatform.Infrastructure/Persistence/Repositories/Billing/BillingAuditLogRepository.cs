using NotaryPlatform.Domain.Features.Billing.Aggregates;
using NotaryPlatform.Domain.Features.Billing.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfBillingAuditLog = NotaryPlatform.Infrastructure.Persistence.Generated.Billing.BillingAuditLog;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Billing;

public sealed class BillingAuditLogRepository : RepositoryBase<EfBillingAuditLog, BillingAuditLog>, IBillingAuditLogRepository
{
    public BillingAuditLogRepository(NotaryPlatformDbContext context) : base(context) { }
}
