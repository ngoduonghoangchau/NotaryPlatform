using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;

namespace NotaryPlatform.Domain.Features.Operations.ValueObjects;

public sealed class ScheduleWindow : ValueObject
{
    public DateTime StartsAt { get; }
    public DateTime EndsAt { get; }

    private ScheduleWindow(DateTime startsAt, DateTime endsAt)
    {
        StartsAt = DateTime.SpecifyKind(startsAt, DateTimeKind.Utc);
        EndsAt = DateTime.SpecifyKind(endsAt, DateTimeKind.Utc);
    }

    public static ScheduleWindow Create(DateTime startsAt, DateTime endsAt)
    {
        if (endsAt < startsAt)
        {
            throw new BusinessRuleValidationException("Schedule window end must be greater than or equal to start.");
        }

        return new ScheduleWindow(startsAt, endsAt);
    }

    public TimeSpan Duration => EndsAt - StartsAt;

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return StartsAt;
        yield return EndsAt;
    }
}
