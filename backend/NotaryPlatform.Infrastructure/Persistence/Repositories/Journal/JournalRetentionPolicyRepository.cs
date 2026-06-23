using NotaryPlatform.Domain.Features.Journal.Aggregates;
using NotaryPlatform.Domain.Features.Journal.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfJournalRetentionPolicy = NotaryPlatform.Infrastructure.Persistence.Generated.Journal.JournalRetentionPolicy;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Journal;

public sealed class JournalRetentionPolicyRepository : RepositoryBase<EfJournalRetentionPolicy, JournalRetentionPolicy>, IJournalRetentionPolicyRepository
{
    public JournalRetentionPolicyRepository(NotaryPlatformDbContext context) : base(context) { }
}
