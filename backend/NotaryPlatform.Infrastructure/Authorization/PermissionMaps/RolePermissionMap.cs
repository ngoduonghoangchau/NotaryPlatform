using static NotaryPlatform.Infrastructure.Authorization.PermissionMaps.PermissionCodes;

namespace NotaryPlatform.Infrastructure.Authorization.PermissionMaps;

/// <summary>
/// Default permission assignments for each built-in role.
/// Used by the database seeder to populate <c>core.role_permissions</c>.
/// Role codes must match the <c>role_code</c> column in <c>core.roles</c>.
///
/// SystemAdmin is not listed here because it is granted access by convention
/// (bypass all policy checks) rather than through an explicit permission list.
/// </summary>
public static class RolePermissionMap
{
    /// <summary>Built-in role code constants.</summary>
    public static class RoleCodes
    {
        /// <summary>Full administrative access within a single tenant.</summary>
        public const string TenantAdmin = "tenant.admin";

        /// <summary>Read-everything + manage compliance, incidents, and audit output.</summary>
        public const string ComplianceOfficer = "compliance.officer";

        /// <summary>Assign Notaries to jobs; manage the scheduling board.</summary>
        public const string Dispatcher = "dispatcher";

        /// <summary>Execute notarial acts and manage own journal entries.</summary>
        public const string Notary = "notary";

        /// <summary>Own B2B customer accounts; create jobs; read billing.</summary>
        public const string AccountManager = "account.manager";

        /// <summary>Manage invoices, payments, and revenue sharing.</summary>
        public const string Finance = "finance";

        /// <summary>Oversee all scheduling, dispatch, and workforce operations.</summary>
        public const string OperationsManager = "operations.manager";
    }

    /// <summary>
    /// Maps each role code to the permission codes it receives by default at seed time.
    /// </summary>
    public static IReadOnlyDictionary<string, IReadOnlyList<string>> DefaultPermissions { get; } =
        new Dictionary<string, IReadOnlyList<string>>
        {
            [RoleCodes.TenantAdmin] = BuildTenantAdminPermissions(),
            [RoleCodes.ComplianceOfficer] = BuildComplianceOfficerPermissions(),
            [RoleCodes.Dispatcher] = BuildDispatcherPermissions(),
            [RoleCodes.Notary] = BuildNotaryPermissions(),
            [RoleCodes.AccountManager] = BuildAccountManagerPermissions(),
            [RoleCodes.Finance] = BuildFinancePermissions(),
            [RoleCodes.OperationsManager] = BuildOperationsManagerPermissions(),
        };

    // ── Role builders ──────────────────────────────────────────────────────────────

    private static IReadOnlyList<string> BuildTenantAdminPermissions() =>
    [
        // CRM — full access
        Crm.CustomersRead, Crm.CustomersCreate, Crm.CustomersUpdate,
        Crm.CustomersDelete, Crm.CustomersExport,
        Crm.ContactsManage, Crm.PricingManage,
        Crm.ContractsRead, Crm.ContractsManage,
        Crm.CommunicationRead, Crm.CommunicationManage,
        Crm.NotesRead, Crm.NotesManage,

        // Notarial — full access
        Notarial.ActsRead, Notarial.ActsCreate, Notarial.ActsUpdate,
        Notarial.ActsExecute, Notarial.ActsLock, Notarial.ActsVoid,
        Notarial.ActsExport, Notarial.CertificatesManage,

        // Journal — full access
        Journal.EntriesRead, Journal.EntriesCreate, Journal.EntriesUpdate,
        Journal.EntriesLock, Journal.EntriesExport,
        Journal.AuditRead, Journal.RetentionManage,

        // Security — full access
        Security.SealsRead, Security.SealsManage,
        Security.SealsSuspend, Security.SealsRevoke,
        Security.CertificatesManage, Security.AccessControlManage,
        Security.IncidentsRead, Security.IncidentsManage, Security.AuditRead,

        // Operations — full access
        Operations.JobsRead, Operations.JobsCreate, Operations.JobsUpdate, Operations.JobsCancel,
        Operations.DispatchRead, Operations.DispatchManage,
        Operations.ScheduleRead, Operations.ScheduleManage,
        Operations.AvailabilityManage, Operations.ReportsRead,

        // Identity — full access
        Identity.NotariesRead, Identity.NotariesCreate, Identity.NotariesUpdate,
        Identity.NotariesSuspend, Identity.NotariesExport,
        Identity.CommissionsManage, Identity.ComplianceRead,

        // Compliance — full access
        Compliance.ChecksRead, Compliance.ChecksManage,
        Compliance.IncidentsRead, Compliance.IncidentsManage,
        Compliance.AuditRead, Compliance.ExportManage, Compliance.RulesManage,

        // Billing — full access
        Billing.InvoicesRead, Billing.InvoicesCreate, Billing.InvoicesUpdate,
        Billing.PaymentsManage, Billing.RevenueRead, Billing.RevenueShareManage,
        Billing.ReportsRead, Billing.AdjustmentsManage,

        // Admin — tenant-scoped subset (no TenantsManage or PermissionsManage)
        Admin.UsersManage, Admin.RolesManage,
        Admin.BranchesManage, Admin.OrgManage, Admin.AuditRead,

        // Core — own-tenant profile and tenant-scoped user/role administration.
        // TenantsCreate/TenantsDelete and Permissions.Manage are platform-level
        // (SystemAdmin-only by convention) and intentionally excluded here.
        Core.TenantsRead, Core.TenantsUpdate,
        Core.UsersRead, Core.UsersCreate, Core.UsersUpdate, Core.UsersLock,
        Core.RolesRead, Core.RolesManage, Core.PermissionsRead,
    ];

    private static IReadOnlyList<string> BuildComplianceOfficerPermissions() =>
    [
        // Read-all for audit purposes
        Crm.CustomersRead, Crm.ContractsRead, Crm.CommunicationRead, Crm.NotesRead,

        // Notarial — read + lock/void/export (no create/update/execute)
        Notarial.ActsRead, Notarial.ActsLock, Notarial.ActsVoid, Notarial.ActsExport,
        Notarial.CertificatesManage,

        // Journal — read + lock + export + retention management
        Journal.EntriesRead, Journal.EntriesLock, Journal.EntriesExport,
        Journal.AuditRead, Journal.RetentionManage,

        // Security — manage suspension/revocation and incidents
        Security.SealsRead, Security.SealsSuspend, Security.SealsRevoke,
        Security.CertificatesManage, Security.AccessControlManage,
        Security.IncidentsRead, Security.IncidentsManage, Security.AuditRead,

        // Operations — read-only visibility
        Operations.JobsRead, Operations.ScheduleRead, Operations.ReportsRead,

        // Identity — read profiles and compliance snapshots; export for audits
        Identity.NotariesRead, Identity.ComplianceRead, Identity.NotariesExport,
        Identity.CommissionsManage,

        // Compliance — full ownership
        Compliance.ChecksRead, Compliance.ChecksManage,
        Compliance.IncidentsRead, Compliance.IncidentsManage,
        Compliance.AuditRead, Compliance.ExportManage, Compliance.RulesManage,

        // Billing — read-only for compliance review
        Billing.InvoicesRead, Billing.RevenueRead, Billing.ReportsRead,

        Admin.AuditRead,
    ];

    private static IReadOnlyList<string> BuildDispatcherPermissions() =>
    [
        // Scheduling & dispatch — primary workspace
        Operations.JobsRead, Operations.JobsCreate, Operations.JobsUpdate, Operations.JobsCancel,
        Operations.DispatchRead, Operations.DispatchManage,
        Operations.ScheduleRead, Operations.ScheduleManage,
        Operations.AvailabilityManage, Operations.ReportsRead,

        // Notary profiles — needed to evaluate eligibility during dispatch
        Identity.NotariesRead, Identity.ComplianceRead,

        // CRM — read customer and contract context for jobs
        Crm.CustomersRead, Crm.ContractsRead,
    ];

    private static IReadOnlyList<string> BuildNotaryPermissions() =>
    [
        // Own-job visibility (resource ownership enforced separately in handlers)
        Operations.JobsRead, Operations.ScheduleRead,
        Operations.AvailabilityManage,

        // Own notarial acts — create and execute, but not lock/void (admin action)
        Notarial.ActsRead, Notarial.ActsCreate, Notarial.ActsUpdate, Notarial.ActsExecute,

        // Own journal entries — create and update; locking is admin-triggered
        Journal.EntriesRead, Journal.EntriesCreate, Journal.EntriesUpdate,

        // Own profile — read-only (editing is admin-only per requirement)
        Identity.NotariesRead,
    ];

    private static IReadOnlyList<string> BuildAccountManagerPermissions() =>
    [
        // CRM — primary ownership
        Crm.CustomersRead, Crm.CustomersCreate, Crm.CustomersUpdate, Crm.CustomersExport,
        Crm.ContactsManage, Crm.PricingManage,
        Crm.ContractsRead, Crm.ContractsManage,
        Crm.CommunicationRead, Crm.CommunicationManage,
        Crm.NotesRead, Crm.NotesManage,

        // Operations — read-only to track job status for clients
        Operations.JobsRead, Operations.ScheduleRead, Operations.ReportsRead,

        // Billing — read invoices and reports for account-level discussions
        Billing.InvoicesRead, Billing.ReportsRead,
    ];

    private static IReadOnlyList<string> BuildFinancePermissions() =>
    [
        // Billing — full ownership
        Billing.InvoicesRead, Billing.InvoicesCreate, Billing.InvoicesUpdate,
        Billing.PaymentsManage, Billing.RevenueRead, Billing.RevenueShareManage,
        Billing.ReportsRead, Billing.AdjustmentsManage,

        // CRM — read customer and contract context needed for invoicing
        Crm.CustomersRead, Crm.ContractsRead,

        // Operations — reports only for revenue reconciliation
        Operations.ReportsRead,
    ];

    private static IReadOnlyList<string> BuildOperationsManagerPermissions() =>
    [
        // Operations — full access
        Operations.JobsRead, Operations.JobsCreate, Operations.JobsUpdate, Operations.JobsCancel,
        Operations.DispatchRead, Operations.DispatchManage,
        Operations.ScheduleRead, Operations.ScheduleManage,
        Operations.AvailabilityManage, Operations.ReportsRead,

        // Identity — manage and update Notary profiles and commissions
        Identity.NotariesRead, Identity.NotariesUpdate,
        Identity.CommissionsManage, Identity.ComplianceRead,

        // Compliance — read compliance status to support operational decisions
        Compliance.ChecksRead,

        // Billing — reports only for operational cost tracking
        Billing.ReportsRead,

        // CRM — customer and contract context
        Crm.CustomersRead, Crm.ContractsRead,
    ];
}
