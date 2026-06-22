using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Operations.Enums;
using NotaryPlatform.Domain.Features.Operations.Events;

namespace NotaryPlatform.Domain.Features.Operations.Aggregates;

public sealed class JobAssignment : AggregateRoot
{
    public Guid JobId { get; private set; }
    public Guid NotaryId { get; private set; }
    public AssignmentRole AssignmentRole { get; private set; }
    public AssignmentStatus Status { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public DateTime? DeclinedAt { get; private set; }
    public DateTime? ReleasedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? Notes { get; private set; }

    private JobAssignment()
    {
    }

    private JobAssignment(Guid id, Guid jobId, Guid notaryId, AssignmentRole assignmentRole)
        : base(id)
    {
        JobId = jobId;
        NotaryId = notaryId;
        AssignmentRole = assignmentRole;
        Status = AssignmentStatus.Proposed;
        AssignedAt = DateTime.UtcNow;
    }

    public static JobAssignment Create(Guid jobId, Guid notaryId, AssignmentRole assignmentRole)
    {
        if (jobId == Guid.Empty) throw new BusinessRuleValidationException("Job id is required.");
        if (notaryId == Guid.Empty) throw new BusinessRuleValidationException("Notary id is required.");

        return new JobAssignment(Guid.NewGuid(), jobId, notaryId, assignmentRole);
    }

    public void Accept()
    {
        Status = AssignmentStatus.Accepted;
        AcceptedAt = DateTime.UtcNow;
    }

    public void Decline(string? reason = null)
    {
        Status = AssignmentStatus.Declined;
        DeclinedAt = DateTime.UtcNow;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }

    public void Release(string? reason = null)
    {
        Status = AssignmentStatus.Released;
        ReleasedAt = DateTime.UtcNow;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }

    public void Replace(Guid newNotaryId, AssignmentRole assignmentRole, string? reason = null)
    {
        if (newNotaryId == Guid.Empty)
        {
            throw new BusinessRuleValidationException("New notary id is required.");
        }

        NotaryId = newNotaryId;
        AssignmentRole = assignmentRole;
        Status = AssignmentStatus.Replaced;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }

    public void Complete()
    {
        Status = AssignmentStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }
}
