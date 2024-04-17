namespace ReToDfa.Regexes.Models.Extensions;

public static class RegexSymbolTypeHelper
{
    public static readonly HashSet<char> SpecialSymbols = new( "()|+*" );

    public static string ToSymbol( this RegexSymbolType type ) => type switch
    {
        RegexSymbolType.And => "&",
        RegexSymbolType.Or => "|",
        RegexSymbolType.ZeroOrMore => "*",
        RegexSymbolType.OneOrMore => "+",
        RegexSymbolType.OpenBrace => "(",
        RegexSymbolType.CloseBrace => ")",
        _ => throw new ArgumentOutOfRangeException( nameof( type ), type, null )
    };
}