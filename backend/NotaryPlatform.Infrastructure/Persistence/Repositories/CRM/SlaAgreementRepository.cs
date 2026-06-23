using NotaryPlatform.Domain.Features.CRM.Aggregates;
using NotaryPlatform.Domain.Features.CRM.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfSlaAgreement = NotaryPlatform.Infrastructure.Persistence.Generated.CRM.SlaAgreement;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.CRM;

public sealed class SlaAgreementRepository : RepositoryBase<EfSlaAgreement, SlaAgreement>, ISlaAgreementRepository
{
    public SlaAgreementRepository(NotaryPlatformDbContext context) : base(context) { }
}
