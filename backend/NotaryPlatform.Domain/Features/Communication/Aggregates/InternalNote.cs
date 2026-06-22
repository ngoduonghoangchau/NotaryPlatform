using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Communication.Enums;

namespace NotaryPlatform.Domain.Features.Communication.Aggregates;

public sealed class InternalNote : AggregateRoot
{
    public Guid CommunicationThreadId { get; private set; }
    public Guid AuthorUserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public NoteVisibility Visibility { get; private set; }
    public bool IsPinned { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? Notes { get; private set; }

    private InternalNote()
    {
    }

    private InternalNote(Guid id, Guid communicationThreadId, Guid authorUserId, string title, string body, NoteVisibility visibility)
        : base(id)
    {
        CommunicationThreadId = communicationThreadId;
        AuthorUserId = authorUserId;
        Title = title;
        Body = body;
        Visibility = visibility;
        CreatedAt = DateTime.UtcNow;
    }

    public static InternalNote Create(
        Guid communicationThreadId,
        Guid authorUserId,
        string title,
        string body,
        NoteVisibility visibility,
        bool isPinned = false,
        string? notes = null)
    {
        if (communicationThreadId == Guid.Empty) throw new BusinessRuleValidationException("Communication thread id is required.");
        if (authorUserId == Guid.Empty) throw new BusinessRuleValidationException("Author user id is required.");
        if (string.IsNullOrWhiteSpace(title)) throw new BusinessRuleValidationException("Internal note title is required.");
        if (string.IsNullOrWhiteSpace(body)) throw new BusinessRuleValidationException("Internal note body is required.");

        return new InternalNote(Guid.NewGuid(), communicationThreadId, authorUserId, title.Trim(), body.Trim(), visibility)
        {
            IsPinned = isPinned,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
}
