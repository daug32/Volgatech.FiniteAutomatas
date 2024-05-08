namespace Grammars.Grammars.LeftRoRightOne.Models.ValueObjects;

public class RuleValue
{
    public readonly IReadOnlyList<RuleSymbol> Symbols;

    public RuleValue( IEnumerable<RuleSymbol> items )
    {
        Symbols = items.ToList();
    }

    public override bool Equals( object? obj ) => obj is RuleValue other && Equals( other );

    public override int GetHashCode()
    {
        return Symbols.GetHashCode();
    }

    public bool Equals( RuleValue other )
    {
        if ( other.Symbols.Count != Symbols.Count )
        {
            return false;
        }

        for ( var i = 0; i < Symbols.Count; i++ )
        {
            RuleSymbol item = Symbols[i];
            RuleSymbol otherItem = other.Symbols[i];

            if ( !item.Equals( otherItem ) )
            {
                return false;
            }
        }

        return true;
    }
    
    public static bool operator == ( RuleValue a, RuleValue b ) => a.Equals( b );

    public static bool operator !=( RuleValue a, RuleValue b ) => !a.Equals( b );

    public override string ToString() => String.Join( "", Symbols );
}