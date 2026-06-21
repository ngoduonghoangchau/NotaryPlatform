namespace NotaryPlatform.Domain.Features.Billing.Enums;

public enum AuditEventType
{
    Create,
    Update,
    IssueInvoice,
    SendInvoice,
    RecordPayment,
    ApplyCredit,
    ApplyRefund,
    PostAdjustment,
    VoidInvoice,
    WriteOff,
    AccruePayable,
    PayPayable,
    StatusChange,
    Export,
    Import
}
