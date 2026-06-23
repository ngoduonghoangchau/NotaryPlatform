using NotaryPlatform.Domain.Features.CRM.Aggregates;
using NotaryPlatform.Domain.Features.CRM.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfCustomerContact = NotaryPlatform.Infrastructure.Persistence.Generated.CRM.CustomerContact;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.CRM;

public sealed class CustomerContactRepository : RepositoryBase<EfCustomerContact, CustomerContact>, ICustomerContactRepository
{
    public CustomerContactRepository(NotaryPlatformDbContext context) : base(context) { }
}
