namespace FiniteAutomatas.Domain.Models.ValueObjects;

public class StateId : IComparable
{
    public readonly string Value;

    public StateId( string value )
    {
        Value = value;
    }

    public StateId( int value ) : this( value.ToString() )
    {
    }

    public override bool Equals( object? obj )
    {
        return obj is StateId other && Equals( other );
    }

    public bool Equals( StateId other )
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
        if ( obj is not StateId other )
        {
            throw new ArgumentException();
        }

        return String.CompareOrdinal( Value, other.Value );
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