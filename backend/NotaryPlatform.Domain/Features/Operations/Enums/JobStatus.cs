namespace NotaryPlatform.Domain.Features.Operations.Enums;

public enum JobStatus
{
    Draft,
    Scheduled,
    Confirmed,
    InProgress,
    Completed,
    Locked,
    Cancelled,
    Failed,
    Archived
}
