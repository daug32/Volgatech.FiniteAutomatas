using Grammars.LL.Models;
using Grammars.LL.Models.ValueObjects;
using Grammars.LL.Models.ValueObjects.Symbols;

namespace Grammars.LL.Console.Displays;

public static class ConsolePrinter
{
    public static LlOneGrammar ToConsole( this LlOneGrammar grammar )
    {
        System.Console.WriteLine( new String( '-', System.Console.WindowWidth ) );
        
        foreach ( GrammarRule rule in grammar.Rules.Values )
        {
            System.Console.WriteLine( $"Rule: \"{rule.Name}\"" );
            foreach ( RuleDefinition ruleDefinition in rule.Definitions )
            {
                string serializedRuleDefinition = String.Join( 
                    "", 
                    ruleDefinition.Symbols.Select( symbol => symbol.Type == RuleSymbolType.NonTerminalSymbol
                        ? $"<{symbol.RuleName}>"
                        : symbol.Symbol!.Value ) );

                System.Console.WriteLine( $"\t\"{serializedRuleDefinition}\"" );
            }
        }

        System.Console.WriteLine( new String( '-', System.Console.WindowWidth ) );

        return grammar;
    }
}