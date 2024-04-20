using FiniteAutomatas.RegularExpressions.Implementation.Models;

namespace FiniteAutomatas.RegularExpressions.Implementation.Extensions;

internal static class RegexSymbolListExtensions
{
    private static readonly Dictionary<RegexSymbolType, int> _symbolTypeToPriority = new()
    {
        { RegexSymbolType.Symbol, 0 },
        { RegexSymbolType.OpenBrace, 0 },
        { RegexSymbolType.CloseBrace, 0 },
        { RegexSymbolType.And, 3 },
        { RegexSymbolType.Or, 2 },
        { RegexSymbolType.ZeroOrMore, 1 },
        { RegexSymbolType.OneOrMore, 1 },
    };

    public static int GetOperationSymbolIndex( this List<RegexSymbol> regex )
    {
        var symbolIndexToPriority = new Dictionary<int, int>();

        int bracesLevel = 0;
        for ( int i = 0; i < regex.Count; i++ )
        {
            RegexSymbol symbol = regex[i];

            if ( symbol.Type == RegexSymbolType.OpenBrace )
            {
                bracesLevel++;
                symbolIndexToPriority[i] = _symbolTypeToPriority[symbol.Type];
                continue;
            }

            if ( symbol.Type == RegexSymbolType.CloseBrace )
            {
                bracesLevel--;
                symbolIndexToPriority[i] = _symbolTypeToPriority[symbol.Type];
                continue;
            }
            
            symbolIndexToPriority[i] = bracesLevel == 0 
                ? _symbolTypeToPriority[symbol.Type] 
                : 0;
        }

        return symbolIndexToPriority.MaxBy( x => x.Value ).Key;
    }
    
    public static List<RegexSymbol> SimplifyBracesIfNeed( this List<RegexSymbol> regex )
    {
        while ( regex.First().Type == RegexSymbolType.OpenBrace && 
                regex.Last().Type == RegexSymbolType.CloseBrace )
        {
            bool needToRemove = true;
            
            int bracesLevel = 0;
            for ( int i = 0; i < regex.Count && needToRemove; i++ )
            {
                if ( regex[i].Type == RegexSymbolType.OpenBrace )
                {
                    bracesLevel += 1;
                    continue;
                }

                if ( regex[i].Type == RegexSymbolType.CloseBrace )
                {
                    bracesLevel -= 1;
                    if ( bracesLevel == 0 && i + 1 < regex.Count - 1 )
                    {
                        needToRemove = false;
                    }
                }
            }

            if ( needToRemove )
            {
                regex.RemoveAt( 0 );
                regex.RemoveAt( regex.Count - 1 );
            }
            else
            {
                break;
            }
        }

        return regex;
    }
}