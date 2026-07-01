using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotaryPlatform.Infrastructure.Persistence.Outbox;

/// <summary>
/// Persisted representation of a domain event waiting to be dispatched via
/// MediatR. Rows are written atomically with the triggering business entities
/// by <c>OutboxSaveChangesInterceptor</c> and consumed by
/// <c>OutboxBackgroundJob</c> on a recurring schedule.
/// <para>
/// Table: <c>public.outbox_messages</c> — created by EF migration.
/// Registered with <see cref="DbContext"/> via
/// <c>NotaryPlatformDbContext.Outbox.cs</c> (partial class).
/// </para>
/// </summary>
[Table("outbox_messages")]
public sealed class OutboxMessage
{
    [Key]
    public Guid Id { get; set; }

    /// <summary>Assembly-qualified CLR type name of the domain event.</summary>
    [Required, MaxLength(500)]
    public string EventType { get; set; } = null!;

    /// <summary>JSON-serialised domain event payload.</summary>
    [Required]
    public string Payload { get; set; } = null!;

    /// <summary>When the domain event was raised (from <c>DomainEvent.OccurredOn</c>).</summary>
    public DateTime OccurredAt { get; set; }

    /// <summary>Set by <c>OutboxMessageProcessor</c> after successful dispatch.</summary>
    public DateTime? ProcessedAt { get; set; }

    /// <summary>Last error message. Null on success.</summary>
    [MaxLength(2000)]
    public string? Error { get; set; }

    /// <summary>Number of dispatch attempts. Capped at <c>OutboxDispatcher.MaxRetries</c>.</summary>
    public int RetryCount { get; set; }
}
