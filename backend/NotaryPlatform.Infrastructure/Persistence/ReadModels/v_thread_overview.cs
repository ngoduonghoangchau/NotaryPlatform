using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Domain.Features.Communication.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.ReadModels;

[Keyless]
public partial class VThreadOverview
{
    public Guid? ThreadId { get; set; }

    public Guid? TenantId { get; set; }

    [StringLength(50)]
    public string? ThreadCode { get; set; }

    [StringLength(300)]
    public string? Subject { get; set; }

    public bool? IsInternal { get; set; }

    public bool? IsImportant { get; set; }

    public Guid? CustomerId { get; set; }

    [StringLength(200)]
    public string? CustomerName { get; set; }

    public Guid? JobId { get; set; }

    public Guid? NotarialActId { get; set; }

    public Guid? InvoiceId { get; set; }

    public Guid? IncidentId { get; set; }

    public DateTime? LastMessageAt { get; set; }

    public DateTime? LastActivityAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public DateTime? ArchivedAt { get; set; }
}

// <auto-enum-partial>
public partial class VThreadOverview
{
    public ChannelType channel_type { get; set; }
    public ThreadStatus thread_status { get; set; }
}
// </auto-enum-partial>
