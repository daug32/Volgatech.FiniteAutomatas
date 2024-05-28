using Grammars.Common.Grammars.ValueObjects.Symbols;

namespace Grammars.Common.Grammars.ValueObjects.RuleDefinitions;

public class RuleDefinition
{
    public readonly IReadOnlyList<RuleSymbol> Symbols;

    public RuleDefinition( IEnumerable<RuleSymbol> items )
    {
        if ( !items.Any() )
        {
            throw new ArgumentException( "Definition must have at least one symbol" );
        }

        Symbols = items.ToList();

        // var symbols = items.ToList();
        //
        // bool hasNonEmptySymbols = symbols.Any( x => x.Type != RuleSymbolType.TerminalSymbol || x.Symbol!.Type != TerminalSymbolType.EmptySymbol );
        //
        // Symbols = hasNonEmptySymbols
        //     ? symbols.Where( x => x.Type != RuleSymbolType.TerminalSymbol || x.Symbol!.Type != TerminalSymbolType.EmptySymbol ).ToList()
        //     : new[] { RuleSymbol.TerminalSymbol( TerminalSymbol.EmptySymbol() ) };
    }

    public RuleSymbol FirstSymbol() => Symbols.First();
    public RuleSymbolType FirstSymbolType() => FirstSymbol().Type;

    public RuleDefinition Copy() => new( Symbols );

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