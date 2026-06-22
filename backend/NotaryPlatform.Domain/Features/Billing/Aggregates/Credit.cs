using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Billing.Enums;
using NotaryPlatform.Domain.Features.Billing.ValueObjects;

namespace NotaryPlatform.Domain.Features.Billing.Aggregates;

public sealed class Credit : AggregateRoot
{
    private readonly List<CreditApplication> _applications = new();

    public Guid TenantId { get; private set; }
    public Guid CustomerId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public CreditStatus Status { get; private set; }
    public Money OriginalAmount { get; private set; } = null!;
    public Money RemainingAmount { get; private set; } = null!;
    public Money? ExpiredAmount { get; private set; }
    public string? Reason { get; private set; }
    public DateTime IssuedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public DateTime? VoidedAt { get; private set; }
    public string? Notes { get; private set; }

    public IReadOnlyCollection<CreditApplication> Applications => _applications.AsReadOnly();

    private Credit()
    {
    }

    private Credit(Guid id, Guid tenantId, Guid customerId, string code, Money originalAmount)
        : base(id)
    {
        TenantId = tenantId;
        CustomerId = customerId;
        Code = code;
        OriginalAmount = originalAmount;
        RemainingAmount = originalAmount;
        Status = CreditStatus.Available;
        IssuedAt = DateTime.UtcNow;
    }

    public static Credit Create(Guid tenantId, Guid customerId, string code, Money originalAmount, DateTime? expiresAt = null, string? reason = null, string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (customerId == Guid.Empty) throw new BusinessRuleValidationException("Customer id is required.");
        if (string.IsNullOrWhiteSpace(code)) throw new BusinessRuleValidationException("Credit code is required.");

        return new Credit(Guid.NewGuid(), tenantId, customerId, code.Trim().ToUpperInvariant(), originalAmount)
        {
            ExpiresAt = expiresAt,
            Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
}
