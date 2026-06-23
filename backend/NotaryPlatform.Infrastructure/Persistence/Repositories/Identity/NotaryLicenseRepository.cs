using NotaryPlatform.Domain.Features.Identity.Aggregates;
using NotaryPlatform.Domain.Features.Identity.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfNotaryLicense = NotaryPlatform.Infrastructure.Persistence.Generated.Identity.NotaryLicense;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Identity;

public sealed class NotaryLicenseRepository : RepositoryBase<EfNotaryLicense, NotaryLicense>, INotaryLicenseRepository
{
    public NotaryLicenseRepository(NotaryPlatformDbContext context) : base(context) { }
}
