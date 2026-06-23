using NotaryPlatform.Domain.Features.Identity.Aggregates;
using NotaryPlatform.Domain.Features.Identity.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfNotaryInsurance = NotaryPlatform.Infrastructure.Persistence.Generated.Identity.NotaryInsurance;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Identity;

public sealed class NotaryInsuranceRepository : RepositoryBase<EfNotaryInsurance, NotaryInsurance>, INotaryInsuranceRepository
{
    public NotaryInsuranceRepository(NotaryPlatformDbContext context) : base(context) { }
}
