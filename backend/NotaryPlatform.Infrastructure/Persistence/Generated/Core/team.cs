using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Generated.Communication;
using NotaryPlatform.Domain.Features.Core.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Generated.Core;

/// <summary>
/// Work teams under branches/regions.
/// </summary>
[Table("teams", Schema = "core")]
[Index("branch_id", Name = "ix_teams_branch_id")]
[Index("region_id", Name = "ix_teams_region_id")]
[Index("tenant_id", Name = "ix_teams_tenant_id")]
[Index("tenant_id", "team_code", Name = "uq_teams_tenant_code", IsUnique = true)]
public partial class Team
{
    [Key]
    public Guid TeamId { get; set; }

    public Guid TenantId { get; set; }

    public Guid BranchId { get; set; }

    public Guid? RegionId { get; set; }

    [StringLength(50)]
    public string TeamCode { get; set; } = null!;

    [StringLength(200)]
    public string TeamName { get; set; } = null!;

    [StringLength(50)]
    public string? TeamType { get; set; }

    [Column(TypeName = "jsonb")]
    public string Settings { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("branch_id")]
    [InverseProperty("teams")]
    public virtual Branch Branch { get; set; } = null!;

    [InverseProperty("assigned_team")]
    public virtual ICollection<CommunicationThread> CommunicationThreads { get; set; } = new List<CommunicationThread>();

    [ForeignKey("region_id")]
    [InverseProperty("teams")]
    public virtual Region? Region { get; set; }

    [ForeignKey("tenant_id")]
    [InverseProperty("teams")]
    public virtual Tenant Tenant { get; set; } = null!;

    [InverseProperty("team")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

// <auto-enum-partial>
public partial class Team
{
    public TeamStatus status { get; set; }
}
// </auto-enum-partial>
