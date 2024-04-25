using FluentAssertions;

namespace FiniteAutomatas.Grammar.LeftToRightOne.Console.Models;

public class GrammarRuleName
{
    public readonly string Value;

    public GrammarRuleName( string value )
    {
        Value = value.ThrowIfNullOrEmpty();
    }

    public override bool Equals( object? obj ) => obj is GrammarRuleName other && Equals( other );

    public bool Equals( GrammarRuleName other ) => other.Value == Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}