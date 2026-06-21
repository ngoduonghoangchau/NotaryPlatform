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
/// Service regions used for dispatch and compliance.
/// </summary>
[Table("regions", Schema = "core")]
[Index("organization_id", Name = "ix_regions_organization_id")]
[Index("state_code", Name = "ix_regions_state_code")]
[Index("tenant_id", Name = "ix_regions_tenant_id")]
[Index("tenant_id", "region_code", Name = "uq_regions_tenant_code", IsUnique = true)]
public partial class Region
{
    [Key]
    public Guid RegionId { get; set; }

    public Guid TenantId { get; set; }

    public Guid OrganizationId { get; set; }

    [StringLength(50)]
    public string RegionCode { get; set; } = null!;

    [StringLength(200)]
    public string RegionName { get; set; } = null!;

    [StringLength(2)]
    public string? CountryCode { get; set; }

    [StringLength(2)]
    public string? StateCode { get; set; }

    public string? Description { get; set; }

    [Column(TypeName = "jsonb")]
    public string Settings { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [InverseProperty("region")]
    public virtual ICollection<AccountsReceivableSnapshot> AccountsReceivableSnapshots { get; set; } = new List<AccountsReceivableSnapshot>();

    [InverseProperty("region")]
    public virtual ICollection<CommunicationThread> CommunicationThreads { get; set; } = new List<CommunicationThread>();

    [InverseProperty("region")]
    public virtual ICollection<ComplianceCheck> ComplianceChecks { get; set; } = new List<ComplianceCheck>();

    [InverseProperty("region")]
    public virtual ICollection<ComplianceRule> ComplianceRules { get; set; } = new List<ComplianceRule>();

    [InverseProperty("region")]
    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    [InverseProperty("region")]
    public virtual ICollection<DailyOperationalSnapshot> DailyOperationalSnapshots { get; set; } = new List<DailyOperationalSnapshot>();

    [InverseProperty("region")]
    public virtual ICollection<DispatchCandidate> DispatchCandidates { get; set; } = new List<DispatchCandidate>();

    [InverseProperty("region")]
    public virtual ICollection<DispatchRule> DispatchRules { get; set; } = new List<DispatchRule>();

    [InverseProperty("region")]
    public virtual ICollection<Incident> Incidents { get; set; } = new List<Incident>();

    [InverseProperty("region")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [InverseProperty("region")]
    public virtual ICollection<JobRequest> JobRequests { get; set; } = new List<JobRequest>();

    [InverseProperty("region")]
    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();

    [InverseProperty("region")]
    public virtual ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();

    [InverseProperty("region")]
    public virtual ICollection<LegalHold> LegalHolds { get; set; } = new List<LegalHold>();

    [InverseProperty("region")]
    public virtual ICollection<Notary> Notaries { get; set; } = new List<Notary>();

    [InverseProperty("region")]
    public virtual ICollection<NotaryAvailabilityRule> NotaryAvailabilityRules { get; set; } = new List<NotaryAvailabilityRule>();

    [InverseProperty("region")]
    public virtual ICollection<NotaryShiftRequest> NotaryShiftRequests { get; set; } = new List<NotaryShiftRequest>();

    [ForeignKey("organization_id")]
    [InverseProperty("regions")]
    public virtual Organization Organization { get; set; } = null!;

    [InverseProperty("region")]
    public virtual ICollection<PolicyVersion> PolicyVersions { get; set; } = new List<PolicyVersion>();

    [InverseProperty("region")]
    public virtual ICollection<ScheduleBlock> ScheduleBlocks { get; set; } = new List<ScheduleBlock>();

    [InverseProperty("region")]
    public virtual ICollection<SealAccessPolicy> SealAccessPolicies { get; set; } = new List<SealAccessPolicy>();

    [InverseProperty("region")]
    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();

    [ForeignKey("tenant_id")]
    [InverseProperty("regions")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class Region
{
    public RegionStatus status { get; set; }
}
// </auto-enum-partial>
