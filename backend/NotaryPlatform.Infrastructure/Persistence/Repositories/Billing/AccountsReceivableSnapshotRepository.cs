using NotaryPlatform.Domain.Features.Billing.Aggregates;
using NotaryPlatform.Domain.Features.Billing.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfAccountsReceivableSnapshot = NotaryPlatform.Infrastructure.Persistence.Generated.Billing.AccountsReceivableSnapshot;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Billing;

public sealed class AccountsReceivableSnapshotRepository : RepositoryBase<EfAccountsReceivableSnapshot, AccountsReceivableSnapshot>, IAccountsReceivableSnapshotRepository
{
    public AccountsReceivableSnapshotRepository(NotaryPlatformDbContext context) : base(context) { }
}
