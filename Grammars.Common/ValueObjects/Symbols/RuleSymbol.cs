using System.Diagnostics;
using FluentAssertions;

namespace Grammars.Common.ValueObjects.Symbols;

/// <summary>
/// Represents a symbol in a grammar rule:<br/>
/// * " " - whitespace<br/>
/// * "BEGIN" - terminal symbol<br/>
/// * "+" - terminal symbol<br/>
/// * &lt;grammarRuleName&gt; - non terminal symbol<br/>
/// </summary>
public class RuleSymbol
{
    public readonly RuleSymbolType Type;
    
    public readonly TerminalSymbol? Symbol;
    public readonly RuleName? RuleName;

    public static RuleSymbol TerminalSymbol( TerminalSymbol symbol ) => new(
        RuleSymbolType.TerminalSymbol, 
        null,
        symbol );

    public static RuleSymbol NonTerminalSymbol( RuleName ruleName ) => new(
        RuleSymbolType.NonTerminalSymbol, 
        ruleName.ThrowIfNull(),
        null );

    private RuleSymbol( 
        RuleSymbolType type,
        RuleName? ruleName, 
        TerminalSymbol? symbol )
    {
        RuleName = ruleName;
        Type = type;
        Symbol = symbol;
    }

    public override bool Equals( object? obj )
    {
        if ( obj is not RuleSymbol other )
        {
            return false;
        }

        if ( other.Type != Type )
        {
            return false;
        }

        if ( Symbol != null )
        {
            return Symbol.Equals( other.Symbol );
        }

        if ( RuleName != null )
        {
            return RuleName.Equals( other.RuleName );
        }

        throw new UnreachableException();
    }

    public static bool operator == ( RuleSymbol a, RuleSymbol b ) => a.Equals( b );

    public static bool operator !=( RuleSymbol a, RuleSymbol b ) => !a.Equals( b );

    public override int GetHashCode() => HashCode.Combine( ( int )Type, Symbol, RuleName );

    public override string ToString() => Type switch
    {
        RuleSymbolType.TerminalSymbol => Symbol?.ToString() ?? throw new UnreachableException(),
        RuleSymbolType.NonTerminalSymbol => RuleName?.ToString() ?? throw new UnreachableException(),
        _ => throw new UnreachableException()
    };
}