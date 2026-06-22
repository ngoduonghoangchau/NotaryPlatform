using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.CRM.Enums;
using NotaryPlatform.Domain.Features.CRM.Events;
using NotaryPlatform.Domain.Features.CRM.ValueObjects;

namespace NotaryPlatform.Domain.Features.CRM.Aggregates;

public sealed class Contract : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public Guid CustomerId { get; private set; }
    public ContractNumber ContractNumber { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public ContractStatus Status { get; private set; }
    public DateOnly SignedOn { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public string? TermsJson { get; private set; }
    public string? Notes { get; private set; }

    private Contract()
    {
    }

    private Contract(Guid id, Guid tenantId, Guid customerId, ContractNumber contractNumber, string name, DateOnly signedOn, DateOnly effectiveFrom)
        : base(id)
    {
        TenantId = tenantId;
        CustomerId = customerId;
        ContractNumber = contractNumber;
        Name = name;
        SignedOn = signedOn;
        EffectiveFrom = effectiveFrom;
        Status = ContractStatus.Draft;
    }

    public static Contract Create(Guid tenantId, Guid customerId, string contractNumber, string name, DateOnly signedOn, DateOnly effectiveFrom)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (customerId == Guid.Empty) throw new BusinessRuleValidationException("Customer id is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessRuleValidationException("Contract name is required.");

        return new Contract(
            Guid.NewGuid(),
            tenantId,
            customerId,
            ContractNumber.Create(contractNumber),
            name.Trim(),
            signedOn,
            effectiveFrom);
    }

    public void UpdateTerms(DateOnly signedOn, DateOnly effectiveFrom, DateOnly? effectiveTo, string? termsJson, string? notes = null)
    {
        if (effectiveTo.HasValue && effectiveTo.Value < effectiveFrom)
        {
            throw new BusinessRuleValidationException("Effective to must be greater than or equal to effective from.");
        }

        SignedOn = signedOn;
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
        TermsJson = termsJson;
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }

    public void Activate()
    {
        Status = ContractStatus.Active;
        AddDomainEvent(new ContractActivatedDomainEvent(Id, CustomerId, ContractNumber.Value));
    }

    public void Renew(DateOnly newEffectiveFrom, DateOnly? newEffectiveTo = null)
    {
        Status = ContractStatus.Renewed;
        EffectiveFrom = newEffectiveFrom;
        EffectiveTo = newEffectiveTo;
    }

    public void Expire() => Status = ContractStatus.Expired;

    public void Terminate(string? reason = null)
    {
        Status = ContractStatus.Terminated;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }

    public void Cancel(string? reason = null)
    {
        Status = ContractStatus.Cancelled;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }
}
