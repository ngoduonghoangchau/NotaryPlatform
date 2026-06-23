using NotaryPlatform.Domain.Features.CRM.Aggregates;
using NotaryPlatform.Domain.Features.CRM.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfContract = NotaryPlatform.Infrastructure.Persistence.Generated.CRM.Contract;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.CRM;

public sealed class ContractRepository : RepositoryBase<EfContract, Contract>, IContractRepository
{
    public ContractRepository(NotaryPlatformDbContext context) : base(context) { }
}
