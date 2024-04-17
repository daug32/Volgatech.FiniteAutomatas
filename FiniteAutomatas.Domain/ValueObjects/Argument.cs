namespace FiniteAutomatas.Domain.ValueObjects;

public class Argument : IComparable
{
    public readonly string Value;

    public static Argument Epsilon => new( "" );

    public Argument( string value )
    {
        Value = value;
    }

    public override bool Equals( object? obj )
    {
        return obj is Argument other && Equals( other );
    }

    public bool Equals( Argument other )
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
        if ( obj is not Argument other )
        {
            throw new ArgumentException();
        }

        return String.CompareOrdinal( Value, other.Value );
    }

    public static bool operator ==( Argument a, Argument b )
    {
        return a.Equals( b );
    }

    public static bool operator !=( Argument a, Argument b )
    {
        return !a.Equals( b );
    }
}