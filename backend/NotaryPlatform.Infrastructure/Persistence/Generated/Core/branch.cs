using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Billing;
using NotaryPlatform.Infrastructure.Persistence.Generated.Communication;
using NotaryPlatform.Infrastructure.Persistence.Generated.Compliance;
using NotaryPlatform.Infrastructure.Persistence.Generated.CRM;
using NotaryPlatform.Infrastructure.Persistence.Generated.Identity;
using NotaryPlatform.Infrastructure.Persistence.Generated.Journal;
using NotaryPlatform.Infrastructure.Persistence.Generated.Operations;
using NotaryPlatform.Infrastructure.Persistence.Generated.Security;
using NotaryPlatform.Domain.Features.Core.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Core;

/// <summary>
/// Operational branches/offices.
/// </summary>
[Table("branches", Schema = "core")]
[Index("organization_id", Name = "ix_branches_organization_id")]
[Index("state_code", Name = "ix_branches_state_code")]
[Index("tenant_id", Name = "ix_branches_tenant_id")]
[Index("tenant_id", "branch_code", Name = "uq_branches_tenant_code", IsUnique = true)]
public partial class Branch
{
    [Key]
    public Guid BranchId { get; set; }

    public Guid TenantId { get; set; }

    public Guid OrganizationId { get; set; }

    [StringLength(50)]
    public string BranchCode { get; set; } = null!;

    [StringLength(200)]
    public string BranchName { get; set; } = null!;

    [StringLength(2)]
    public string? StateCode { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(200)]
    public string? AddressLine1 { get; set; }

    [StringLength(200)]
    public string? AddressLine2 { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }

    [StringLength(2)]
    public string? CountryCode { get; set; }

    [StringLength(64)]
    public string TimeZone { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string Settings { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("branch")]
    public virtual ICollection<AccountsReceivableSnapshot> AccountsReceivableSnapshots { get; set; } = new List<AccountsReceivableSnapshot>();

    [InverseProperty("branch")]
    public virtual ICollection<CommunicationThread> CommunicationThreads { get; set; } = new List<CommunicationThread>();

    [InverseProperty("branch")]
    public virtual ICollection<ComplianceCheck> ComplianceChecks { get; set; } = new List<ComplianceCheck>();

    [InverseProperty("branch")]
    public virtual ICollection<ComplianceRule> ComplianceRules { get; set; } = new List<ComplianceRule>();

    [InverseProperty("branch")]
    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    [InverseProperty("branch")]
    public virtual ICollection<DailyOperationalSnapshot> DailyOperationalSnapshots { get; set; } = new List<DailyOperationalSnapshot>();

    [InverseProperty("branch")]
    public virtual ICollection<DispatchCandidate> DispatchCandidates { get; set; } = new List<DispatchCandidate>();

    [InverseProperty("branch")]
    public virtual ICollection<DispatchRule> DispatchRules { get; set; } = new List<DispatchRule>();

    [InverseProperty("branch")]
    public virtual ICollection<Incident> Incidents { get; set; } = new List<Incident>();

    [InverseProperty("branch")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [InverseProperty("branch")]
    public virtual ICollection<JobRequest> JobRequests { get; set; } = new List<JobRequest>();

    [InverseProperty("branch")]
    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();

    [InverseProperty("branch")]
    public virtual ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();

    [InverseProperty("branch")]
    public virtual ICollection<LegalHold> LegalHolds { get; set; } = new List<LegalHold>();

    [InverseProperty("branch")]
    public virtual ICollection<Notary> Notaries { get; set; } = new List<Notary>();

    [InverseProperty("branch")]
    public virtual ICollection<NotaryAvailabilityRule> NotaryAvailabilityRules { get; set; } = new List<NotaryAvailabilityRule>();

    [InverseProperty("branch")]
    public virtual ICollection<NotaryShiftRequest> NotaryShiftRequests { get; set; } = new List<NotaryShiftRequest>();

    [ForeignKey("organization_id")]
    [InverseProperty("branches")]
    public virtual Organization Organization { get; set; } = null!;

    [InverseProperty("branch")]
    public virtual ICollection<PolicyVersion> PolicyVersions { get; set; } = new List<PolicyVersion>();

    [InverseProperty("branch")]
    public virtual ICollection<ScheduleBlock> ScheduleBlocks { get; set; } = new List<ScheduleBlock>();

    [InverseProperty("branch")]
    public virtual ICollection<SealAccessPolicy> SealAccessPolicies { get; set; } = new List<SealAccessPolicy>();

    [InverseProperty("branch")]
    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();

    [ForeignKey("tenant_id")]
    [InverseProperty("branches")]
    public virtual Tenant Tenant { get; set; } = null!;

    [InverseProperty("branch")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

// <auto-enum-partial>
public partial class Branch
{
    public BranchStatus status { get; set; }
}
// </auto-enum-partial>
