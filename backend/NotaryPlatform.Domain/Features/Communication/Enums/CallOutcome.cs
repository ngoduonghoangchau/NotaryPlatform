namespace NotaryPlatform.Domain.Features.Communication.Enums;

public enum CallOutcome
{
    Connected,
    NoAnswer,
    VoicemailLeft,
    CallbackRequested,
    FollowUpNeeded,
    Resolved,
    Escalated,
    Other
}
