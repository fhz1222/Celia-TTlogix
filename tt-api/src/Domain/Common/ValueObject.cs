namespace Domain.Common;

// Learn more: https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/microservice-ddd-cqrs-patterns/implement-value-objects
public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();

    public static bool operator ==(ValueObject? one, ValueObject? two) => EqualOperator(one, two);

    public static bool operator !=(ValueObject? one, ValueObject? two) => !EqualOperator(one, two);

    protected static bool EqualOperator(ValueObject? left, ValueObject? right)
    {
        if (left is null ^ right is null) { return false; }
        return left?.Equals(right!) != false;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType()) { return false; }

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
        => GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
}