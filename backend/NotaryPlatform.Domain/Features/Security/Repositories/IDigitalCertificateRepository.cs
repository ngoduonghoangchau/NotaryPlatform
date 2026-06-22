using NotaryPlatform.Domain.Features.Security.Aggregates;
using NotaryPlatform.Domain.Features.Security.ValueObjects;

namespace NotaryPlatform.Domain.Features.Security.Repositories;

public interface IDigitalCertificateRepository
{
    Task<DigitalCertificate?> GetByIdAsync(Guid digitalCertificateId, CancellationToken cancellationToken = default);
    Task<DigitalCertificate?> GetBySerialNumberAsync(CertificateSerialNumber serialNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DigitalCertificate>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DigitalCertificate>> ListByNotaryAsync(Guid notaryId, CancellationToken cancellationToken = default);
    Task AddAsync(DigitalCertificate certificate, CancellationToken cancellationToken = default);
    Task UpdateAsync(DigitalCertificate certificate, CancellationToken cancellationToken = default);
    Task DeleteAsync(DigitalCertificate certificate, CancellationToken cancellationToken = default);
}
