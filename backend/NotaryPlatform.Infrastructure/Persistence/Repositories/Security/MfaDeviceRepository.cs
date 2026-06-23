using NotaryPlatform.Domain.Features.Security.Aggregates;
using NotaryPlatform.Domain.Features.Security.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfMfaDevice = NotaryPlatform.Infrastructure.Persistence.Generated.Security.MfaDevice;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Security;

public sealed class MfaDeviceRepository : RepositoryBase<EfMfaDevice, MfaDevice>, IMfaDeviceRepository
{
    public MfaDeviceRepository(NotaryPlatformDbContext context) : base(context) { }
}
