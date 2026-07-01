using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Outbox;

namespace NotaryPlatform.Infrastructure.Persistence.DbContexts;

/// <summary>
/// Partial-class extension that registers <see cref="OutboxMessage"/> with
/// the EF Core model via the scaffold-generated <c>OnModelCreatingPartial</c>
/// hook (called at the end of <c>OnModelCreating</c> in the generated file).
/// Run <c>dotnet ef migrations add AddOutboxMessages</c> to create the table.
/// </summary>
public partial class NotaryPlatformDbContext
{
    /// <summary>Access point for outbox queries inside <see cref="OutboxDispatcher"/>.</summary>
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("outbox_messages"); // public schema (no prefix)

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedNever(); // set by interceptor

            entity.Property(e => e.EventType)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Payload)
                .IsRequired();

            entity.Property(e => e.OccurredAt)
                .IsRequired();

            entity.Property(e => e.RetryCount)
                .HasDefaultValue(0);

            entity.HasIndex(e => e.ProcessedAt)
                .HasDatabaseName("ix_outbox_messages_processed_at");

            entity.HasIndex(e => new { e.ProcessedAt, e.RetryCount })
                .HasDatabaseName("ix_outbox_messages_pending")
                .HasFilter("processed_at IS NULL");
        });
    }
}
