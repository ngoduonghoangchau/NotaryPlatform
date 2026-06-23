using NotaryPlatform.Domain.Features.CRM.Aggregates;
using NotaryPlatform.Domain.Features.CRM.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfCustomerSegment = NotaryPlatform.Infrastructure.Persistence.Generated.CRM.CustomerSegment;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.CRM;

public sealed class CustomerSegmentRepository : RepositoryBase<EfCustomerSegment, CustomerSegment>, ICustomerSegmentRepository
{
    public CustomerSegmentRepository(NotaryPlatformDbContext context) : base(context) { }
}
