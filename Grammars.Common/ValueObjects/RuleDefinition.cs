using Grammars.Common.ValueObjects.Symbols;

namespace Grammars.Common.ValueObjects;

public class RuleDefinition
{
    public readonly IReadOnlyList<RuleSymbol> Symbols;

    public RuleDefinition( IEnumerable<RuleSymbol> items )
    {
        Symbols = items.ToList();
    }

    public override bool Equals( object? obj ) => obj is RuleDefinition other && Equals( other );

    public override int GetHashCode()
    {
        return Symbols.GetHashCode();
    }

    public bool Equals( RuleDefinition other )
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
    
    public static bool operator == ( RuleDefinition a, RuleDefinition b ) => a.Equals( b );

    public static bool operator !=( RuleDefinition a, RuleDefinition b ) => !a.Equals( b );

    public override string ToString() => String.Join( "", Symbols );
}