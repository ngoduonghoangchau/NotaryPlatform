using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Operations.Enums;
using NotaryPlatform.Domain.Features.Operations.Events;

namespace NotaryPlatform.Domain.Features.Operations.Aggregates;

public sealed class ServiceType : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public ServiceMode DefaultMode { get; private set; }
    public bool IsActive { get; private set; }
    public string? Notes { get; private set; }

    private ServiceType()
    {
    }

    private ServiceType(Guid id, Guid tenantId, string code, string name, ServiceMode defaultMode, string? description, string? notes)
        : base(id)
    {
        TenantId = tenantId;
        Code = code;
        Name = name;
        DefaultMode = defaultMode;
        Description = description;
        Notes = notes;
        IsActive = true;
    }

    public static ServiceType Create(Guid tenantId, string code, string name, ServiceMode defaultMode, string? description = null, string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (string.IsNullOrWhiteSpace(code)) throw new BusinessRuleValidationException("Service type code is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessRuleValidationException("Service type name is required.");

        var serviceType = new ServiceType(
            Guid.NewGuid(),
            tenantId,
            code.Trim().ToUpperInvariant(),
            name.Trim(),
            defaultMode,
            string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            string.IsNullOrWhiteSpace(notes) ? null : notes.Trim());

        serviceType.AddDomainEvent(new ServiceTypeCreatedDomainEvent(serviceType.Id, tenantId, serviceType.Code));
        return serviceType;
    }

    public void UpdateProfile(string name, ServiceMode defaultMode, string? description = null, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleValidationException("Service type name is required.");
        }

        Name = name.Trim();
        DefaultMode = defaultMode;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
