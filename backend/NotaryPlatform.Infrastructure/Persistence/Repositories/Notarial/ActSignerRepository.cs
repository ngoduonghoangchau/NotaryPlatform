using NotaryPlatform.Domain.Features.Notarial.Aggregates;
using NotaryPlatform.Domain.Features.Notarial.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfActSigner = NotaryPlatform.Infrastructure.Persistence.Generated.Notarial.ActSigner;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Notarial;

public sealed class ActSignerRepository : RepositoryBase<EfActSigner, ActSigner>, IActSignerRepository
{
    public ActSignerRepository(NotaryPlatformDbContext context) : base(context) { }
}
