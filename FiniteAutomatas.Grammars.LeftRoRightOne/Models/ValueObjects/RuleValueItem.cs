using System.Diagnostics;
using FluentAssertions;

namespace FiniteAutomatas.Grammars.LeftRoRightOne.Models.ValueObjects;

/// <summary>
/// Represents a symbol in a grammar rule:<br/>
/// * " " - whitespace<br/>
/// * "BEGIN" - terminal symbol<br/>
/// * "+" - terminal symbol<br/>
/// * &lt;grammarRuleName&gt; - non terminal symbol<br/>
/// </summary>
public class RuleValueItem
{
    public readonly RuleValueItemType Type;
    public readonly string? Word;
    public readonly RuleName? RuleName;

    public static RuleValueItem WhiteSpace() => new( 
        RuleValueItemType.WhiteSpace, 
        null, 
        null );

    public static RuleValueItem TerminalSymbol( string word ) => new(
        RuleValueItemType.TerminalSymbol, 
        null,
        word.ThrowIfNullOrWhiteSpace() );

    public static RuleValueItem TerminalSymbol( char letter ) => new(
        RuleValueItemType.TerminalSymbol, 
        null,
        Char.IsWhiteSpace( letter ) 
            ? throw new ArgumentException( "RuleValueItem can not be terminal symbol if the letter is a whitespace" )
            : letter.ToString() );

    public static RuleValueItem NonTerminalSymbol( RuleName ruleName ) => new(
        RuleValueItemType.NonTerminalSymbol, 
        ruleName.ThrowIfNull(),
        null );

    private RuleValueItem( 
        RuleValueItemType type,
        RuleName? ruleName, 
        string? word )
    {
        RuleName = ruleName;
        Type = type;
        Word = word;
    }

    public override bool Equals( object? obj ) => obj is RuleValueItem other && Equals( other );

    public bool Equals( RuleValueItem other ) => 
        Type == other.Type &&
        Word == other.Word &&
        Equals( RuleName, other.RuleName );
    
    public static bool operator == ( RuleValueItem a, RuleValueItem b ) => a.Equals( b );

    public static bool operator !=( RuleValueItem a, RuleValueItem b ) => !a.Equals( b );

    public override int GetHashCode() => HashCode.Combine( ( int )Type, Word, RuleName );

    public override string ToString() => Type switch
    {
        RuleValueItemType.TerminalSymbol => Word ?? throw new UnreachableException(),
        RuleValueItemType.WhiteSpace => @"<\s+>",
        RuleValueItemType.NonTerminalSymbol => ( RuleName ?? throw new UnreachableException() ).ToString(),
        _ => throw new UnreachableException()
    };
}