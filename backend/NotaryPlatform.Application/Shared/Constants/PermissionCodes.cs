namespace NotaryPlatform.Application.Shared.Constants;

/// <summary>
/// Source-of-truth catalog of every permission code recognised by the system.
/// Values must match the <c>permission_code</c> column in <c>core.permissions</c>.
/// Format: <c>{module}.{resource}.{action}</c>
/// </summary>
public static class PermissionCodes
{
    /// <summary>
    /// CRM module — customer lifecycle, contacts, contracts, pricing, communication, and notes.
    /// Corresponds to the Customer Relationship Management function.
    /// </summary>
    public static class Crm
    {
        // Customers
        public const string CustomersRead = "crm.customers.read";
        public const string CustomersCreate = "crm.customers.create";
        public const string CustomersUpdate = "crm.customers.update";
        public const string CustomersDelete = "crm.customers.delete";
        public const string CustomersExport = "crm.customers.export";

        // Contacts (B2B contact people within an account)
        public const string ContactsManage = "crm.contacts.manage";

        // Pricing rules and volume discounts
        public const string PricingManage = "crm.pricing.manage";

        // Contracts & SLA
        public const string ContractsRead = "crm.contracts.read";
        public const string ContractsManage = "crm.contracts.manage";

        // Communication log
        public const string CommunicationRead = "crm.communication.read";
        public const string CommunicationManage = "crm.communication.manage";

        // Internal notes and documents
        public const string NotesRead = "crm.notes.read";
        public const string NotesManage = "crm.notes.manage";
    }

    /// <summary>
    /// Notarial module — full lifecycle of a notarial act: setup, execution, certificate, lock, void.
    /// </summary>
    public static class Notarial
    {
        public const string ActsRead = "notarial.acts.read";
        public const string ActsCreate = "notarial.acts.create";
        public const string ActsUpdate = "notarial.acts.update";
        public const string ActsExecute = "notarial.acts.execute";  // perform the physical act
        public const string ActsLock = "notarial.acts.lock";
        public const string ActsVoid = "notarial.acts.void";
        public const string ActsExport = "notarial.acts.export";

        public const string CertificatesManage = "notarial.certificates.manage";
    }

    /// <summary>
    /// Journal module — state-compliant electronic notary journal.
    /// Immutability enforcement: entries cannot be edited after locking.
    /// </summary>
    public static class Journal
    {
        public const string EntriesRead = "journal.entries.read";
        public const string EntriesCreate = "journal.entries.create";
        public const string EntriesUpdate = "journal.entries.update";
        public const string EntriesLock = "journal.entries.lock";
        public const string EntriesExport = "journal.entries.export";

        public const string AuditRead = "journal.audit.read";
        public const string RetentionManage = "journal.retention.manage";
    }

    /// <summary>
    /// Security module — notary seals, electronic seals, and digital certificates.
    /// High-risk legal assets requiring strict access control and full traceability.
    /// </summary>
    public static class Security
    {
        // Physical and electronic seals
        public const string SealsRead = "security.seals.read";
        public const string SealsManage = "security.seals.manage";
        public const string SealsSuspend = "security.seals.suspend";
        public const string SealsRevoke = "security.seals.revoke";

        // Digital certificates (CA, HSM, key management)
        public const string CertificatesManage = "security.certificates.manage";

        // Access control rules for seal/signature usage
        public const string AccessControlManage = "security.access-control.manage";

        // Incidents: lost seals, compromised keys, revocation
        public const string IncidentsRead = "security.incidents.read";
        public const string IncidentsManage = "security.incidents.manage";

        public const string AuditRead = "security.audit.read";
    }

    /// <summary>
    /// Operations module — scheduling, dispatch, job management, and workforce coordination.
    /// Covers the Scheduling &amp; Dispatch function and Notary availability management.
    /// </summary>
    public static class Operations
    {
        // Jobs / appointments
        public const string JobsRead = "operations.jobs.read";
        public const string JobsCreate = "operations.jobs.create";
        public const string JobsUpdate = "operations.jobs.update";
        public const string JobsCancel = "operations.jobs.cancel";

        // Dispatch — assigning Notaries to jobs
        public const string DispatchRead = "operations.dispatch.read";
        public const string DispatchManage = "operations.dispatch.manage";

        // Master scheduling board and calendars
        public const string ScheduleRead = "operations.schedule.read";
        public const string ScheduleManage = "operations.schedule.manage";

        // Notary availability rules and shift requests
        public const string AvailabilityManage = "operations.availability.manage";

        // Operational reports (KPIs, on-time rate, reassignment rate)
        public const string ReportsRead = "operations.reports.read";
    }

    /// <summary>
    /// Identity module — Notary profiles, commissions, bonds, insurance, and credentials.
    /// Central to determining whether a Notary is legally eligible for an assignment.
    /// </summary>
    public static class Identity
    {
        public const string NotariesRead = "identity.notaries.read";
        public const string NotariesCreate = "identity.notaries.create";
        public const string NotariesUpdate = "identity.notaries.update";
        public const string NotariesSuspend = "identity.notaries.suspend";
        public const string NotariesExport = "identity.notaries.export";

        // Commission lifecycle (active, expiring, renewal tracking)
        public const string CommissionsManage = "identity.commissions.manage";

        // Compliance snapshot on notary profile
        public const string ComplianceRead = "identity.compliance.read";
    }

    /// <summary>
    /// Compliance module — automated checks, incidents, regulatory exports, and audit trails.
    /// The Centralized Compliance Engine that gates all legally sensitive operations.
    /// </summary>
    public static class Compliance
    {
        public const string ChecksRead = "compliance.checks.read";
        public const string ChecksManage = "compliance.checks.manage";

        public const string IncidentsRead = "compliance.incidents.read";
        public const string IncidentsManage = "compliance.incidents.manage";

        public const string AuditRead = "compliance.audit.read";
        public const string ExportManage = "compliance.export.manage";

        // Compliance rule configuration (state-specific rule engine)
        public const string RulesManage = "compliance.rules.manage";
    }

    /// <summary>
    /// Billing module — invoices, payments, revenue sharing, and financial reporting.
    /// Covers fee calculation, accounts receivable, and Notary commission payables.
    /// </summary>
    public static class Billing
    {
        public const string InvoicesRead = "billing.invoices.read";
        public const string InvoicesCreate = "billing.invoices.create";
        public const string InvoicesUpdate = "billing.invoices.update";

        public const string PaymentsManage = "billing.payments.manage";

        public const string RevenueRead = "billing.revenue.read";
        public const string RevenueShareManage = "billing.revenue-share.manage";

        public const string ReportsRead = "billing.reports.read";

        // Credits and billing adjustments
        public const string AdjustmentsManage = "billing.adjustments.manage";
    }

    /// <summary>
    /// Admin module — user management, role/permission administration,
    /// tenant provisioning, and org-structure (branches, regions, teams).
    /// System-level operations not scoped to a single tenant.
    /// </summary>
    public static class Admin
    {
        public const string UsersManage = "admin.users.manage";
        public const string RolesManage = "admin.roles.manage";
        public const string PermissionsManage = "admin.permissions.manage";
        public const string TenantsManage = "admin.tenants.manage";
        public const string BranchesManage = "admin.branches.manage";
        public const string OrgManage = "admin.org.manage";
        public const string AuditRead = "admin.audit.read";
    }

    /// <summary>
    /// Core module — fine-grained CRUD over the <c>core</c> schema aggregates
    /// (tenants, users, roles, permissions). Tenant provisioning
    /// (<see cref="TenantsCreate"/>/<see cref="TenantsDelete"/>) and permission
    /// catalog authoring (<see cref="PermissionsManage"/>) are platform-level
    /// operations granted to SystemAdmin by convention, not through
    /// <c>RolePermissionMap</c>.
    /// </summary>
    public static class Core
    {
        // Tenants
        public const string TenantsRead = "core.tenants.read";
        public const string TenantsCreate = "core.tenants.create";
        public const string TenantsUpdate = "core.tenants.update";
        public const string TenantsDelete = "core.tenants.delete";

        // Users
        public const string UsersRead = "core.users.read";
        public const string UsersCreate = "core.users.create";
        public const string UsersUpdate = "core.users.update";
        public const string UsersLock = "core.users.lock";

        // Roles
        public const string RolesRead = "core.roles.read";
        public const string RolesManage = "core.roles.manage";

        // Permissions
        public const string PermissionsRead = "core.permissions.read";
        public const string PermissionsManage = "core.permissions.manage";
    }
}
