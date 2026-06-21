using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Domain.Features.Communication.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.ReadModels;

[Keyless]
public partial class VPendingReminder
{
    public Guid? ReminderId { get; set; }

    public Guid? TenantId { get; set; }

    [StringLength(50)]
    public string? ReminderCode { get; set; }

    public DateTime? ScheduledAt { get; set; }

    public Guid? JobId { get; set; }

    public Guid? NotarialActId { get; set; }

    public Guid? JournalEntryId { get; set; }

    public Guid? InvoiceId { get; set; }
}

// <auto-enum-partial>
public partial class VPendingReminder
{
    public ChannelType channel_type { get; set; }
    public ReminderStatus reminder_status { get; set; }
}
// </auto-enum-partial>
