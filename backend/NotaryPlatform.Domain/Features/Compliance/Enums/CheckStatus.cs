namespace NotaryPlatform.Domain.Features.Compliance.Enums;

public enum CheckStatus
{
    Pending,
    Running,
    Passed,
    Failed,
    Warning,
    Blocked,
    Skipped,
    ManualReview
}
