using NotaryPlatform.Domain.Features.Notarial.Aggregates;
using NotaryPlatform.Domain.Features.Notarial.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfActExecutionRecord = NotaryPlatform.Infrastructure.Persistence.Generated.Notarial.ActExecutionRecord;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Notarial;

public sealed class ActExecutionRecordRepository : RepositoryBase<EfActExecutionRecord, ActExecutionRecord>, IActExecutionRecordRepository
{
    public ActExecutionRecordRepository(NotaryPlatformDbContext context) : base(context) { }
}
