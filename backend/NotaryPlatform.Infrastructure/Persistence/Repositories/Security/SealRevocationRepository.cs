using NotaryPlatform.Domain.Features.Security.Aggregates;
using NotaryPlatform.Domain.Features.Security.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfSealRevocation = NotaryPlatform.Infrastructure.Persistence.Generated.Security.SealRevocation;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Security;

public sealed class SealRevocationRepository : RepositoryBase<EfSealRevocation, SealRevocation>, ISealRevocationRepository
{
    public SealRevocationRepository(NotaryPlatformDbContext context) : base(context) { }
}
