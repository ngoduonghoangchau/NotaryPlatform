using NotaryPlatform.Domain.Features.Compliance.Aggregates;
using NotaryPlatform.Domain.Features.Compliance.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfRegulatoryExport = NotaryPlatform.Infrastructure.Persistence.Generated.Compliance.RegulatoryExport;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Compliance;

public sealed class RegulatoryExportRepository : RepositoryBase<EfRegulatoryExport, RegulatoryExport>, IRegulatoryExportRepository
{
    public RegulatoryExportRepository(NotaryPlatformDbContext context) : base(context) { }
}
