using System.Linq;

namespace NotaryPlatform.Domain.Common.Base;

public abstract class ValueObject
{
    protected abstract IEnumerable<object?> GetAtomicValues();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;
        return GetAtomicValues().SequenceEqual(other.GetAtomicValues());
    }

    public override int GetHashCode()
    {
        return GetAtomicValues()
            .Aggregate(0, (current, value) => HashCode.Combine(current, value));
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
        => Equals(left, right);

    public static bool operator !=(ValueObject? left, ValueObject? right)
        => !Equals(left, right);
}
