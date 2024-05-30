using Grammars.Common.Grammars;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.Symbols;

namespace Grammars.Visualization;

public static class ConsolePrinter
{
    public static T ToConsole<T>( this T grammar, string? stepDescription = null ) where T : CommonGrammar
    {
        var splitter = new string( '-', Console.WindowWidth - 1 );

        Console.WriteLine( splitter );
        if ( stepDescription != null )
        {
            Console.WriteLine( stepDescription );
        }

        foreach ( GrammarRule grammarRule in grammar.Rules.Values )
        {
            string serializedDefinitions = String.Join(
                " | ",
                grammarRule.Definitions
                    .Select( definition =>
                    {
                        var serializedSymbols = definition.Symbols
                            .Where( s => s.Type != RuleSymbolType.TerminalSymbol || s.Symbol.Type != TerminalSymbolType.WhiteSpace )
                            .Select( s => 
                                s.Type == RuleSymbolType.NonTerminalSymbol 
                                    ? $"<{s.RuleName}>"
                                    : s.Symbol.ToString() );
                        return String.Join( " ", serializedSymbols );
                    } )
                    .OrderByDescending( x => x.Length )
                    .ThenBy( x => x ) );
            Console.WriteLine( $"<{grammarRule.Name}> -> {serializedDefinitions}" );
        }

        Console.WriteLine( splitter );

        return grammar;
    }
}