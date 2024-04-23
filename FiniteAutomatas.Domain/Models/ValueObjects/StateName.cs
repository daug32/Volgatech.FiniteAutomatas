namespace FiniteAutomatas.Domain.Models.ValueObjects;

public class StateName : IComparable
{
    public readonly string Value;

    public StateName( string value )
    {
        Value = value;
    }

    public override bool Equals( object? obj )
    {
        return obj is StateName other && Equals( other );
    }

    public bool Equals( StateName other )
    {
        return Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }

    public int CompareTo( object? obj )
    {
        if ( obj is not StateName other )
        {
            throw new ArgumentException();
        }

        return String.CompareOrdinal( Value, other.Value );
    }

    public static bool operator ==( StateName a, StateName b )
    {
        return a.Equals( b );
    }

    public static bool operator !=( StateName a, StateName b )
    {
        return !a.Equals( b );
    }
}