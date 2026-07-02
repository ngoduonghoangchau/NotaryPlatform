namespace NotaryPlatform.Application.Shared.Exceptions;

/// <summary>
/// Thrown when a user attempts to log in while their account is temporarily locked
/// due to too many failed attempts (BR-AUTH-02). Maps to HTTP 423 Locked.
/// </summary>
public sealed class AccountLockedException : Exception
{
    public TimeSpan RemainingLockout { get; }

    public AccountLockedException(DateTime lockedUntilUtc)
        : base(BuildMessage(lockedUntilUtc))
    {
        var remaining = lockedUntilUtc - DateTime.UtcNow;
        RemainingLockout = remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }

    private static string BuildMessage(DateTime lockedUntilUtc)
    {
        var remaining = lockedUntilUtc - DateTime.UtcNow;
        return remaining.TotalMinutes >= 1
            ? $"Account is temporarily locked. Try again in {(int)Math.Ceiling(remaining.TotalMinutes)} minutes."
            : $"Account is temporarily locked. Try again in {Math.Max((int)remaining.TotalSeconds, 1)} seconds.";
    }
}
