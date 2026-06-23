using NotaryPlatform.Domain.Features.Security.Aggregates;
using NotaryPlatform.Domain.Features.Security.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfDigitalCertificate = NotaryPlatform.Infrastructure.Persistence.Generated.Security.DigitalCertificate;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Security;

public sealed class DigitalCertificateRepository : RepositoryBase<EfDigitalCertificate, DigitalCertificate>, IDigitalCertificateRepository
{
    public DigitalCertificateRepository(NotaryPlatformDbContext context) : base(context) { }
}
