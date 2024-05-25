using Grammars.Common;
using Grammars.Common.ValueObjects;
using Grammars.Common.ValueObjects.Symbols;

namespace Grammars.Console.Displays;

public static class ConsolePrinter
{
    public static T ToConsole<T>( this T grammar ) where T : CommonGrammar
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
                        : symbol.Symbol!.ToString() ) );

                System.Console.WriteLine( $"\t\"{serializedRuleDefinition}\"" );
            }
        }

        System.Console.WriteLine( new String( '-', System.Console.WindowWidth ) );

        return grammar;
    }
}