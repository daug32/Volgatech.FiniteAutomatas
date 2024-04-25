using FiniteAutomatas.Grammar.LeftToRightOne.Console.Parsers;

namespace FiniteAutomatas.Grammar.LeftToRightOne.Console;

public class Program
{
    public static void Main( string[] args )
    {
        var grammarParser = new GrammarRulesParser( 
            @"D:\Development\Projects\FiniteAutomatas\FiniteAutomatas.Grammar.LeftToRightOne.Console\Grammars\common.txt" );
        var rules = grammarParser.Parse();
    }
}