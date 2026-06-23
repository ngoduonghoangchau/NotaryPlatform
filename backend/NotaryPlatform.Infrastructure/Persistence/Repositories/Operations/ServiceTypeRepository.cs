using NotaryPlatform.Domain.Features.Operations.Aggregates;
using NotaryPlatform.Domain.Features.Operations.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfServiceType = NotaryPlatform.Infrastructure.Persistence.Generated.Operations.ServiceType;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Operations;

public sealed class ServiceTypeRepository : RepositoryBase<EfServiceType, ServiceType>, IServiceTypeRepository
{
    public ServiceTypeRepository(NotaryPlatformDbContext context) : base(context) { }
}
