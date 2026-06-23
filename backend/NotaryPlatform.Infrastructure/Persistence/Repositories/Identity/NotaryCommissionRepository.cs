using NotaryPlatform.Domain.Features.Identity.Aggregates;
using NotaryPlatform.Domain.Features.Identity.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfNotaryCommission = NotaryPlatform.Infrastructure.Persistence.Generated.Identity.NotaryCommission;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Identity;

public sealed class NotaryCommissionRepository : RepositoryBase<EfNotaryCommission, NotaryCommission>, INotaryCommissionRepository
{
    public NotaryCommissionRepository(NotaryPlatformDbContext context) : base(context) { }
}
