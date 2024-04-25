using Grammars.Grammars.LeftRoRightOne.Models;
using Grammars.Grammars.LeftRoRightOne.Models.ValueObjects;
using Grammars.LL.Console.Parsers;

namespace Grammars.LL.Console;

public class Program
{
    private static readonly GrammarParser _grammarParser = new();
    
    public static void Main()
    {
        Grammar grammar = _grammarParser.ParseFromFile(
            @"D:\Development\Projects\FiniteAutomatas\Grammars.LL.Console\Grammars\common.txt" );

        foreach ( GrammarRule rule in grammar.Rules.Values )
        {
            System.Console.WriteLine( $"Rule: \"{rule.Name}\"" );
            foreach ( RuleValue ruleValue in rule.Values )
            {
                System.Console.WriteLine( $"\t\"{ruleValue}\"" );
            }
        }
    }
}