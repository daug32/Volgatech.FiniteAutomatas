using FiniteAutomatas.RegularExpressions.Implementation.Extensions;

namespace FiniteAutomatas.RegularExpressions.Implementation.Models;

internal class RegexSymbol
{
    public readonly char? Value;
    public readonly RegexSymbolType Type;

    private RegexSymbol( char value )
    {
        Value = value;
        Type = RegexSymbolType.Symbol;
    }

    private RegexSymbol( RegexSymbolType symbolType )
    {
        Type = symbolType == RegexSymbolType.Symbol
            ? throw new InvalidOperationException()
            : symbolType;
    }

    public static List<RegexSymbol> Parse( string regex )
    {
        var result = new List<RegexSymbol>( regex.Length );

        for ( var i = 0; i < regex.Length; i++ )
        {
            RegexSymbolType currentSymbolType = ParseRegexSymbolType( regex[i] );
            RegexSymbolType? nextSymbolType = i + 1 < regex.Length
                ? ParseRegexSymbolType( regex[i + 1] )
                : null;
            
            if ( currentSymbolType is RegexSymbolType.Symbol )
            {
                result.Add( new RegexSymbol( regex[i] ) );

                if ( nextSymbolType is 
                    RegexSymbolType.Symbol or 
                    RegexSymbolType.OpenBrace )
                {
                    result.Add( new RegexSymbol( RegexSymbolType.And ) );
                }
            }
            else
            {
                result.Add( new RegexSymbol( currentSymbolType ) );

                if ( currentSymbolType is
                         RegexSymbolType.CloseBrace or 
                         RegexSymbolType.OneOrMore or 
                         RegexSymbolType.ZeroOrMore &&
                     nextSymbolType is
                         RegexSymbolType.Symbol )
                {
                    result.Add( new RegexSymbol( RegexSymbolType.And ) );
                    continue;
                }

                if ( currentSymbolType is RegexSymbolType.CloseBrace && nextSymbolType is RegexSymbolType.OpenBrace )
                {
                    result.Add( new RegexSymbol( RegexSymbolType.And ) );
                }
            }
        }

        return result;
    }

    public override string ToString()
    {
        return Type == RegexSymbolType.Symbol
            ? Value!.ToString()!
            : Type.ToSymbol();
    }

    private static RegexSymbolType ParseRegexSymbolType( char c )
    {
        return c switch
        {
            '(' => RegexSymbolType.OpenBrace,
            ')' => RegexSymbolType.CloseBrace,
            '+' => RegexSymbolType.OneOrMore,
            '*' => RegexSymbolType.ZeroOrMore,
            '|' => RegexSymbolType.Or,
            _ => RegexSymbolType.Symbol
        };
    }
}