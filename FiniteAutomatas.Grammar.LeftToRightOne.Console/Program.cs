using FiniteAutomatas.Grammar.LeftToRightOne.Console.Models;
using FiniteAutomatas.Grammar.LeftToRightOne.Console.Parsers;

namespace FiniteAutomatas.Grammar.LeftToRightOne.Console;

public class Program
{
    public static void Main( string[] args )
    {
        var grammarParser = new GrammarRulesParser( 
            @"D:\Development\Projects\FiniteAutomatas\FiniteAutomatas.Grammar.LeftToRightOne.Console\Grammars\common.txt" );
        var rules = grammarParser.Parse();

        foreach ( var rule in rules.Values )
        {
            System.Console.WriteLine( $"Rule: \"{rule.Name}\"" );
            foreach ( GrammarRuleValue ruleValue in rule.Values )
            {
                System.Console.WriteLine( $"\t\"{ruleValue}\"" );
            }
        }
    }
}