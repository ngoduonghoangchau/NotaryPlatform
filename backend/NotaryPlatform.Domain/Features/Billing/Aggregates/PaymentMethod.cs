using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Billing.Enums;

namespace NotaryPlatform.Domain.Features.Billing.Aggregates;

public sealed class PaymentMethod : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public Guid CustomerId { get; private set; }
    public PaymentMethodType MethodType { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? ProviderName { get; private set; }
    public string? Token { get; private set; }
    public string? MaskedAccount { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? VerifiedAt { get; private set; }
    public DateTime? ExpiredAt { get; private set; }

    private PaymentMethod()
    {
    }

    private PaymentMethod(Guid id, Guid tenantId, Guid customerId, PaymentMethodType methodType)
        : base(id)
    {
        TenantId = tenantId;
        CustomerId = customerId;
        MethodType = methodType;
        Status = PaymentStatus.Pending;
    }

    public static PaymentMethod Create(Guid tenantId, Guid customerId, PaymentMethodType methodType, string? providerName = null, string? token = null, string? maskedAccount = null, string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (customerId == Guid.Empty) throw new BusinessRuleValidationException("Customer id is required.");

        return new PaymentMethod(Guid.NewGuid(), tenantId, customerId, methodType)
        {
            ProviderName = string.IsNullOrWhiteSpace(providerName) ? null : providerName.Trim(),
            Token = string.IsNullOrWhiteSpace(token) ? null : token.Trim(),
            MaskedAccount = string.IsNullOrWhiteSpace(maskedAccount) ? null : maskedAccount.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
}
