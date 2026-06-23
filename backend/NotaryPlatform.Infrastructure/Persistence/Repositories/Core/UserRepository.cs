using NotaryPlatform.Domain.Features.Core.Aggregates;
using NotaryPlatform.Domain.Features.Core.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfUser = NotaryPlatform.Infrastructure.Persistence.Generated.Core.User;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Core;

public sealed class UserRepository : RepositoryBase<EfUser, User>, IUserRepository
{
    public UserRepository(NotaryPlatformDbContext context) : base(context) { }
}
