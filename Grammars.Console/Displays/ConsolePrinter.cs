using Grammars.Common;
using Grammars.Common.ValueObjects;
using Grammars.Common.ValueObjects.Symbols;

namespace Grammars.Console.Displays;

public static class ConsolePrinter
{
    public static T ToConsole<T>( this T grammar, string? stepDescription = null ) where T : CommonGrammar
    {
        var splitter = new String( '-', System.Console.WindowWidth - 1 );
        
        System.Console.WriteLine( splitter );
        if ( stepDescription != null )
        {
            System.Console.WriteLine( stepDescription );
        }
        
        foreach ( GrammarRule rule in grammar.Rules.Values )
        {
            System.Console.WriteLine( $"Rule: \"{rule.Name}\"" );
            foreach ( RuleDefinition ruleDefinition in rule.Definitions )
            {
                string serializedRuleDefinition = String.Join( 
                    "", 
                    ruleDefinition.Symbols.Select( symbol => symbol.Type == RuleSymbolType.NonTerminalSymbol
                        ? $"<{symbol.RuleName}>"
                        : symbol.Symbol!.ToString() ) );

                System.Console.WriteLine( $"\t\"{serializedRuleDefinition}\"" );
            }
        }

        System.Console.WriteLine( splitter );

        return grammar;
    }
}