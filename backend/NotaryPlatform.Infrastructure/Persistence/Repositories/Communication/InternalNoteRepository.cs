using NotaryPlatform.Domain.Features.Communication.Aggregates;
using NotaryPlatform.Domain.Features.Communication.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfInternalNote = NotaryPlatform.Infrastructure.Persistence.Generated.Communication.InternalNote;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Communication;

public sealed class InternalNoteRepository : RepositoryBase<EfInternalNote, InternalNote>, IInternalNoteRepository
{
    public InternalNoteRepository(NotaryPlatformDbContext context) : base(context) { }
}
