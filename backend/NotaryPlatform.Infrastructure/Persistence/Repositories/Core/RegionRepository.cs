using NotaryPlatform.Domain.Features.Core.Aggregates;
using NotaryPlatform.Domain.Features.Core.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfRegion = NotaryPlatform.Infrastructure.Persistence.Generated.Core.Region;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Core;

public sealed class RegionRepository : RepositoryBase<EfRegion, Region>, IRegionRepository
{
    public RegionRepository(NotaryPlatformDbContext context) : base(context) { }
}
