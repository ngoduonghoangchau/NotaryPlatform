using NotaryPlatform.Application.Abstractions.System;

namespace NotaryPlatform.Infrastructure.Services;

internal sealed class SystemDateTimeService : IDateTime
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    public DateOnly UtcToday => DateOnly.FromDateTime(DateTime.UtcNow);
}
