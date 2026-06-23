using NotaryPlatform.Domain.Features.Compliance.Aggregates;
using NotaryPlatform.Domain.Features.Compliance.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfLegalHold = NotaryPlatform.Infrastructure.Persistence.Generated.Compliance.LegalHold;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Compliance;

public sealed class LegalHoldRepository : RepositoryBase<EfLegalHold, LegalHold>, ILegalHoldRepository
{
    public LegalHoldRepository(NotaryPlatformDbContext context) : base(context) { }
}
