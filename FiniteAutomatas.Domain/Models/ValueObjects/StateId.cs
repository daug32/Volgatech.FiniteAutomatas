namespace FiniteAutomatas.Domain.Models.ValueObjects;

public class StateId : IComparable
{
    public readonly int Value;

    public StateId( int value )
    {
        Value = value;
    }

    public override bool Equals( object? obj ) => obj is StateId other && Equals( other );

    public bool Equals( StateId other ) => Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public int CompareTo( object? obj )
    {
        if ( obj is not StateId other )
        {
            throw new ArgumentException();
        }

        return Value.CompareTo( other.Value );
    }

    public static bool operator ==( StateId a, StateId b )
    {
        return a.Equals( b );
    }

    public static bool operator !=( StateId a, StateId b )
    {
        return !a.Equals( b );
    }
}