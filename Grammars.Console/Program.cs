using Grammars.Common.Convertors;
using Grammars.Common.Convertors.LeftFactorization;
using Grammars.Console.Parsers;
using Grammars.Console.Displays;
using Grammars.LL.Convertors;

namespace Grammars.Console;

public class Program
{
    private static readonly GrammarParser _grammarParser = new();
    
    public static void Main()
    {
        var grammar = _grammarParser
            .ParseFile( @"../../../Grammars/common.txt" )
            .ToConsole()
            // .Convert( new ToLlOneGrammarConvertor() )
            // .ToConsole()
            .Convert( new LeftFactorizationConvertor() );
    }
}