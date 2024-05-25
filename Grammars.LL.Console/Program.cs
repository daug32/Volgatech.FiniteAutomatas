using Grammars.LL.Console.Displays;
using Grammars.LL.Console.Parsers;
using Grammars.LL.Convertors;
using Grammars.LL.Convertors.LeftRecursions;
using Grammars.LL.Models;

namespace Grammars.LL.Console;

public class Program
{
    private static readonly GrammarParser _grammarParser = new();
    
    public static void Main()
    {
        LlOneGrammar grammar = _grammarParser
            .ParseFile( @"../../../Grammars/common.txt" )
            .ToConsole()
            .Convert( new LeftRecursionRemoverConvertor() )
            .ToConsole();
    }
}