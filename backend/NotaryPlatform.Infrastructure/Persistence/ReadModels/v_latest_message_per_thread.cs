using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Domain.Features.Communication.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.ReadModels;

[Keyless]
public partial class VLatestMessagePerThread
{
    public Guid? MessageId { get; set; }

    public Guid? TenantId { get; set; }

    public Guid? ThreadId { get; set; }

    [StringLength(50)]
    public string? MessageCode { get; set; }

    [StringLength(300)]
    public string? Subject { get; set; }

    public DateTime? SentAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public DateTime? ReadAt { get; set; }

    public bool? IsImportant { get; set; }

    public bool? IsInternal { get; set; }
}

// <auto-enum-partial>
public partial class VLatestMessagePerThread
{
    public MessageDirection direction { get; set; }
    public MessageStatus message_status { get; set; }
}
// </auto-enum-partial>
