using Grammars.Common;
using Grammars.Common.ValueObjects;
using Grammars.Common.ValueObjects.Symbols;

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

        foreach ( GrammarRule rule in grammar.Rules.Values )
        {
            Console.WriteLine( $"Rule: \"{rule.Name}\"" );
            foreach ( RuleDefinition ruleDefinition in rule.Definitions )
            {
                string serializedRuleDefinition = String.Join(
                    "",
                    ruleDefinition.Symbols.Select( symbol => symbol.Type == RuleSymbolType.NonTerminalSymbol
                        ? $"<{symbol.RuleName}>"
                        : symbol.Symbol!.ToString() ) );

                Console.WriteLine( $"\t\"{serializedRuleDefinition}\"" );
            }
        }

        Console.WriteLine( splitter );

        return grammar;
    }
}