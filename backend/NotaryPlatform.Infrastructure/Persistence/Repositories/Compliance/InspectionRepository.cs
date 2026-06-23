using NotaryPlatform.Domain.Features.Compliance.Aggregates;
using NotaryPlatform.Domain.Features.Compliance.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfInspection = NotaryPlatform.Infrastructure.Persistence.Generated.Compliance.Inspection;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Compliance;

public sealed class InspectionRepository : RepositoryBase<EfInspection, Inspection>, IInspectionRepository
{
    public InspectionRepository(NotaryPlatformDbContext context) : base(context) { }
}
