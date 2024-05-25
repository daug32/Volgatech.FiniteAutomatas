using Grammars.Grammars.LeftRoRightOne.Models;
using Grammars.Grammars.LeftRoRightOne.Models.ValueObjects;
using Grammars.LL.Console.Parsers;

namespace Grammars.LL.Console;

public class Program
{
    private static readonly GrammarParser _grammarParser = new();
    
    public static void Main()
    {
        LlOneGrammar llOneGrammar = _grammarParser.ParseFile( @"../../../Grammars/common.txt" );

        foreach ( GrammarRule rule in llOneGrammar.Rules.Values )
        {
            System.Console.WriteLine( $"Rule: \"{rule.Name}\"" );
            foreach ( RuleDefinition ruleValue in rule.Definitions )
            {
                System.Console.WriteLine( $"\t\"{ruleValue}\"" );
            }
        }
    }
}