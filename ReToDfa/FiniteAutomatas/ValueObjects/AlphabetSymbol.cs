namespace ReToDfa.FiniteAutomatas.ValueObjects;

public class AlphabetSymbol
{
    public readonly string Value;

    public static AlphabetSymbol Epsilon => new( "" );

    public AlphabetSymbol( string value )
    {
        Value = value;
    }

    public override string ToString() => Value;

    public override bool Equals( object? obj )
    {
        return obj is AlphabetSymbol other && Equals( other );
    }

    public bool Equals( AlphabetSymbol other )
    {
        return Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==( AlphabetSymbol a, AlphabetSymbol b ) => a.Equals( b );

    public static bool operator !=( AlphabetSymbol a, AlphabetSymbol b ) => !a.Equals( b );
}