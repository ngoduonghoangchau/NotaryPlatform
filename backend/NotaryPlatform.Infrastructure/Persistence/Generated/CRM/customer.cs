using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Billing;
using NotaryPlatform.Infrastructure.Persistence.Generated.Communication;
using NotaryPlatform.Infrastructure.Persistence.Generated.Compliance;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;
using NotaryPlatform.Infrastructure.Persistence.Generated.Notarial;
using NotaryPlatform.Infrastructure.Persistence.Generated.Operations;
using NotaryPlatform.Domain.Features.CRM.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.CRM;

/// <summary>
/// Core customer/account entity for B2C and B2B.
/// </summary>
[Table("customers", Schema = "crm")]
[Index("account_manager_user_id", Name = "ix_customers_account_manager_user_id")]
[Index("branch_id", Name = "ix_customers_branch_id")]
[Index("legal_name", Name = "ix_customers_legal_name")]
[Index("region_id", Name = "ix_customers_region_id")]
[Index("tenant_id", Name = "ix_customers_tenant_id")]
[Index("tenant_id", "customer_code", Name = "uq_customers_tenant_code", IsUnique = true)]
public partial class Customer
{
    [Key]
    public Guid CustomerId { get; set; }

    public Guid TenantId { get; set; }

    [StringLength(50)]
    public string CustomerCode { get; set; } = null!;

    [StringLength(200)]
    public string DisplayName { get; set; } = null!;

    [StringLength(250)]
    public string? LegalName { get; set; }

    [StringLength(50)]
    public string? TaxId { get; set; }

    [StringLength(120)]
    public string? Industry { get; set; }

    [StringLength(250)]
    public string? Website { get; set; }

    [Column(TypeName = "citext")]
    public string? PrimaryEmail { get; set; }

    [StringLength(30)]
    public string? PrimaryPhone { get; set; }

    [Column(TypeName = "citext")]
    public string? BillingEmail { get; set; }

    [StringLength(30)]
    public string? BillingPhone { get; set; }

    public Guid? AccountManagerUserId { get; set; }

    public Guid? BranchId { get; set; }

    public Guid? RegionId { get; set; }

    [StringLength(200)]
    public string? AddressLine1 { get; set; }

    [StringLength(200)]
    public string? AddressLine2 { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(2)]
    public string? StateCode { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }

    [StringLength(2)]
    public string? CountryCode { get; set; }

    public int TotalJobs { get; set; }

    [Precision(18, 2)]
    public decimal TotalRevenue { get; set; }

    public int? AvgTurnaroundMinutes { get; set; }

    [Column(TypeName = "jsonb")]
    public string TagsSummary { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string Settings { get; set; } = null!;

    public string? NotesSummary { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("account_manager_user_id")]
    [InverseProperty("customers")]
    public virtual User? AccountManagerUser { get; set; }

    [InverseProperty("customer")]
    public virtual ICollection<AccountsReceivableSnapshot> AccountsReceivableSnapshots { get; set; } = new List<AccountsReceivableSnapshot>();

    [InverseProperty("customer")]
    public virtual ICollection<BillingAdjustment> BillingAdjustments { get; set; } = new List<BillingAdjustment>();

    [ForeignKey("branch_id")]
    [InverseProperty("customers")]
    public virtual Branch? Branch { get; set; }

    [InverseProperty("customer")]
    public virtual ICollection<CommunicationParticipant> CommunicationParticipants { get; set; } = new List<CommunicationParticipant>();

    [InverseProperty("customer")]
    public virtual ICollection<CommunicationThread> CommunicationThreads { get; set; } = new List<CommunicationThread>();

    [InverseProperty("customer")]
    public virtual ICollection<ComplianceCheck> ComplianceChecks { get; set; } = new List<ComplianceCheck>();

    [InverseProperty("customer")]
    public virtual ICollection<ComplianceRule> ComplianceRules { get; set; } = new List<ComplianceRule>();

    [InverseProperty("customer")]
    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    [InverseProperty("customer")]
    public virtual ICollection<Credit> Credits { get; set; } = new List<Credit>();

    [InverseProperty("customer")]
    public virtual CustomerContact? CustomerContact { get; set; }

    [InverseProperty("customer")]
    public virtual ICollection<CustomerDocument> CustomerDocuments { get; set; } = new List<CustomerDocument>();

    [InverseProperty("customer")]
    public virtual ICollection<CustomerNote> CustomerNotes { get; set; } = new List<CustomerNote>();

    [InverseProperty("customer")]
    public virtual ICollection<CustomerSegmentAssignment> CustomerSegmentAssignments { get; set; } = new List<CustomerSegmentAssignment>();

    [InverseProperty("customer")]
    public virtual ICollection<CustomerStatusHistory> CustomerStatusHistories { get; set; } = new List<CustomerStatusHistory>();

    [InverseProperty("customer")]
    public virtual ICollection<CustomerTagAssignment> CustomerTagAssignments { get; set; } = new List<CustomerTagAssignment>();

    [InverseProperty("customer")]
    public virtual ICollection<Incident> Incidents { get; set; } = new List<Incident>();

    [InverseProperty("customer")]
    public virtual ICollection<InternalNote> InternalNotes { get; set; } = new List<InternalNote>();

    [InverseProperty("customer")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [InverseProperty("customer")]
    public virtual ICollection<JobRequest> JobRequests { get; set; } = new List<JobRequest>();

    [InverseProperty("customer")]
    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();

    [InverseProperty("customer")]
    public virtual ICollection<LegalHold> LegalHolds { get; set; } = new List<LegalHold>();

    [InverseProperty("customer")]
    public virtual ICollection<NotarialAct> NotarialActs { get; set; } = new List<NotarialAct>();

    [InverseProperty("customer")]
    public virtual PaymentMethod? PaymentMethod { get; set; }

    [InverseProperty("customer")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [InverseProperty("customer")]
    public virtual ICollection<PricingPlan> PricingPlans { get; set; } = new List<PricingPlan>();

    [InverseProperty("customer")]
    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();

    [ForeignKey("region_id")]
    [InverseProperty("customers")]
    public virtual Region? Region { get; set; }

    [InverseProperty("customer")]
    public virtual ICollection<RevenueShare> RevenueShares { get; set; } = new List<RevenueShare>();

    [ForeignKey("tenant_id")]
    [InverseProperty("customers")]
    public virtual Tenant Tenant { get; set; } = null!;
}

// <auto-enum-partial>
public partial class Customer
{
    public CustomerType customer_type { get; set; }
    public CustomerStatus status { get; set; }
}
// </auto-enum-partial>
