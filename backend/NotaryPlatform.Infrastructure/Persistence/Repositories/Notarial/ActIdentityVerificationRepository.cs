using NotaryPlatform.Domain.Features.Notarial.Aggregates;
using NotaryPlatform.Domain.Features.Notarial.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfActIdentityVerification = NotaryPlatform.Infrastructure.Persistence.Generated.Notarial.ActIdentityVerification;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Notarial;

public sealed class ActIdentityVerificationRepository : RepositoryBase<EfActIdentityVerification, ActIdentityVerification>, IActIdentityVerificationRepository
{
    public ActIdentityVerificationRepository(NotaryPlatformDbContext context) : base(context) { }
}
