namespace NotaryPlatform.Application.Abstractions.System;

/// <summary>
/// Abstracts the system clock so that date/time-dependent logic can be
/// tested deterministically without mocking the static DateTime/DateTimeOffset.
/// </summary>
public interface IDateTime
{
    /// <summary>Current UTC date and time.</summary>
    DateTimeOffset UtcNow { get; }

    /// <summary>Current UTC date (time component is midnight).</summary>
    DateOnly UtcToday { get; }
}
