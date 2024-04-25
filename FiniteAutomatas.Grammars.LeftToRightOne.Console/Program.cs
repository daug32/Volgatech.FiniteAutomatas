using FiniteAutomatas.Grammars.LeftRoRightOne.Models;
using FiniteAutomatas.Grammars.LeftRoRightOne.Models.ValueObjects;
using FiniteAutomatas.Grammars.LeftToRightOne.Console.Parsers;

namespace FiniteAutomatas.Grammars.LeftToRightOne.Console;

public class Program
{
    private static readonly GrammarParser _grammarParser = new();
    
    public static void Main()
    {
        Grammar grammar = _grammarParser.ParseFromFile(
            @"D:\Development\Projects\FiniteAutomatas\FiniteAutomatas.Grammars.LeftToRightOne.Console\Grammars\common.txt" );

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