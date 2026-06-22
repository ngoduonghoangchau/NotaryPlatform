using NotaryPlatform.Domain.Features.Notarial.Aggregates;

namespace NotaryPlatform.Domain.Features.Notarial.Repositories;

public interface INotarialCertificateRepository
{
    Task<NotarialCertificate?> GetByIdAsync(Guid notarialCertificateId, CancellationToken cancellationToken = default);
    Task<NotarialCertificate?> GetByActIdAsync(Guid notarialActId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotarialCertificate>> ListByActIdAsync(Guid notarialActId, CancellationToken cancellationToken = default);
    Task AddAsync(NotarialCertificate certificate, CancellationToken cancellationToken = default);
    Task UpdateAsync(NotarialCertificate certificate, CancellationToken cancellationToken = default);
    Task DeleteAsync(NotarialCertificate certificate, CancellationToken cancellationToken = default);
}
