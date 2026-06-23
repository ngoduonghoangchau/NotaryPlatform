using NotaryPlatform.Domain.Features.Security.Aggregates;
using NotaryPlatform.Domain.Features.Security.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfSealReplacement = NotaryPlatform.Infrastructure.Persistence.Generated.Security.SealReplacement;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Security;

public sealed class SealReplacementRepository : RepositoryBase<EfSealReplacement, SealReplacement>, ISealReplacementRepository
{
    public SealReplacementRepository(NotaryPlatformDbContext context) : base(context) { }
}
