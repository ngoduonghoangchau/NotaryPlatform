using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Billing.ValueObjects;

namespace NotaryPlatform.Domain.Features.Billing.Aggregates;

public sealed class AccountsReceivableSnapshot : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public Guid CustomerId { get; private set; }
    public DateOnly SnapshotDate { get; private set; }
    public Money TotalInvoiced { get; private set; } = null!;
    public Money TotalPaid { get; private set; } = null!;
    public Money TotalOutstanding { get; private set; } = null!;
    public Money? TotalOverdue { get; private set; }
    public string? Notes { get; private set; }

    private AccountsReceivableSnapshot()
    {
    }

    private AccountsReceivableSnapshot(Guid id, Guid tenantId, Guid customerId, DateOnly snapshotDate, Money totalInvoiced, Money totalPaid, Money totalOutstanding)
        : base(id)
    {
        TenantId = tenantId;
        CustomerId = customerId;
        SnapshotDate = snapshotDate;
        TotalInvoiced = totalInvoiced;
        TotalPaid = totalPaid;
        TotalOutstanding = totalOutstanding;
    }

    public static AccountsReceivableSnapshot Create(Guid tenantId, Guid customerId, DateOnly snapshotDate, Money totalInvoiced, Money totalPaid, Money totalOutstanding, Money? totalOverdue = null, string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (customerId == Guid.Empty) throw new BusinessRuleValidationException("Customer id is required.");

        return new AccountsReceivableSnapshot(Guid.NewGuid(), tenantId, customerId, snapshotDate, totalInvoiced, totalPaid, totalOutstanding)
        {
            TotalOverdue = totalOverdue,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
}
