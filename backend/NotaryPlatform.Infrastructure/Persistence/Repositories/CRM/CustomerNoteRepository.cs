using NotaryPlatform.Domain.Features.CRM.Aggregates;
using NotaryPlatform.Domain.Features.CRM.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfCustomerNote = NotaryPlatform.Infrastructure.Persistence.Generated.CRM.CustomerNote;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.CRM;

public sealed class CustomerNoteRepository : RepositoryBase<EfCustomerNote, CustomerNote>, ICustomerNoteRepository
{
    public CustomerNoteRepository(NotaryPlatformDbContext context) : base(context) { }
}
