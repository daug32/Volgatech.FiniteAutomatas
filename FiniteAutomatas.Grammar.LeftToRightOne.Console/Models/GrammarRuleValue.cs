namespace FiniteAutomatas.Grammar.LeftToRightOne.Console.Models;

public class GrammarRuleValue
{
    public readonly string Value;

    public GrammarRuleValue( string value )
    {
        Value = value;
    }

    public override bool Equals( object? obj ) => obj is GrammarRuleName other && Equals( other );

    public bool Equals( GrammarRuleName other ) => other.Value == Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}