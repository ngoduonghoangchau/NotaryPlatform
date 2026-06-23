using NotaryPlatform.Domain.Features.Notarial.Aggregates;
using NotaryPlatform.Domain.Features.Notarial.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfNotarialCertificate = NotaryPlatform.Infrastructure.Persistence.Generated.Notarial.NotarialCertificate;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Notarial;

public sealed class NotarialCertificateRepository : RepositoryBase<EfNotarialCertificate, NotarialCertificate>, INotarialCertificateRepository
{
    public NotarialCertificateRepository(NotaryPlatformDbContext context) : base(context) { }
}
