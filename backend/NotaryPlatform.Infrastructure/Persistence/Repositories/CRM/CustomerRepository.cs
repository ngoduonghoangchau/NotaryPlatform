using NotaryPlatform.Domain.Features.CRM.Aggregates;
using NotaryPlatform.Domain.Features.CRM.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfCustomer = NotaryPlatform.Infrastructure.Persistence.Generated.CRM.Customer;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.CRM;

public sealed class CustomerRepository : RepositoryBase<EfCustomer, Customer>, ICustomerRepository
{
    public CustomerRepository(NotaryPlatformDbContext context) : base(context) { }
}
