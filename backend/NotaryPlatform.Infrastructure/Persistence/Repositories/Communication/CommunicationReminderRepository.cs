using NotaryPlatform.Domain.Features.Communication.Aggregates;
using NotaryPlatform.Domain.Features.Communication.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfCommunicationReminder = NotaryPlatform.Infrastructure.Persistence.Generated.Communication.CommunicationReminder;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Communication;

public sealed class CommunicationReminderRepository : RepositoryBase<EfCommunicationReminder, CommunicationReminder>, ICommunicationReminderRepository
{
    public CommunicationReminderRepository(NotaryPlatformDbContext context) : base(context) { }
}
