using NotaryPlatform.Domain.Features.Security.Aggregates;
using NotaryPlatform.Domain.Features.Security.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfTrustedDevice = NotaryPlatform.Infrastructure.Persistence.Generated.Security.TrustedDevice;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Security;

public sealed class TrustedDeviceRepository : RepositoryBase<EfTrustedDevice, TrustedDevice>, ITrustedDeviceRepository
{
    public TrustedDeviceRepository(NotaryPlatformDbContext context) : base(context) { }
}
