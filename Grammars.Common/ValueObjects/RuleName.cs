using FluentAssertions;

namespace Grammars.Common.ValueObjects;

public class RuleName
{
    public const char RuleNameOpenSymbol = '<';
    public const char RuleNameCloseSymbol = '>';
    
    public readonly string Value;

    public RuleName( string value )
    {
        value = value.ThrowIfNullOrWhiteSpace();
        if ( value.StartsWith( RuleNameOpenSymbol ) )
        {
            value = value.Substring( 1, value.Length - 1 );
        }

        if ( value.EndsWith( RuleNameCloseSymbol ) )
        {
            value = value.Substring( 0, value.Length - 1 );
        }

        Value = value;
    }

    public override bool Equals( object? obj ) => obj is RuleName other && other.Value.Equals( Value );
    
    public static bool operator == ( RuleName? a, RuleName? b ) => a?.Equals( b ) ?? b == null;

    public static bool operator !=( RuleName? a, RuleName? b ) => !a?.Equals( b ) ?? b == null;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}