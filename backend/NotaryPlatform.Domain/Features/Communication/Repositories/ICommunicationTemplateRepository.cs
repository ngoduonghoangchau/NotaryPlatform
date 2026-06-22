using NotaryPlatform.Domain.Features.Communication.Aggregates;

namespace NotaryPlatform.Domain.Features.Communication.Repositories;

public interface ICommunicationTemplateRepository
{
    Task<CommunicationTemplate?> GetByIdAsync(Guid communicationTemplateId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CommunicationTemplate>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(CommunicationTemplate template, CancellationToken cancellationToken = default);
    Task UpdateAsync(CommunicationTemplate template, CancellationToken cancellationToken = default);
    Task DeleteAsync(CommunicationTemplate template, CancellationToken cancellationToken = default);
}
