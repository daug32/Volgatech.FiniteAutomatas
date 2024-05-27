using Grammars.Common.Convertors;
using Grammars.Console.Parsers;
using Grammars.LL.Convertors;
using Grammars.LL.Models;
using Grammars.Visualization;

namespace Grammars.Console;

public class Program
{
    private static readonly string _exitCommand = "exit";
    
    private static readonly GrammarParser _grammarParser = new();
    
    public static void Main()
    {
        AskForSentences( BuildGrammar() );
    }

    private static LlOneGrammar BuildGrammar() => _grammarParser
        .ParseFile( @"../../../Grammars/common.txt" )
        .Convert( new RemoveWhitespacesConvertor() )
        .ToConsole( "Original grammar" )
        .Convert( new ToLlOneGrammarConvertor() )
        .ToConsole( "LL one grammar" );

    private static void AskForSentences( LlOneGrammar grammar )
    {
        while ( true )
        {
            System.Console.Write( $"Enter sentence or type \"{_exitCommand}\" to exit: " );

            string command = System.Console.ReadLine() ?? String.Empty;
            if ( _exitCommand.Equals( command, StringComparison.InvariantCultureIgnoreCase ) )
            {
                System.Console.WriteLine( "Exiting" );
                return;
            }

            RunResult result = grammar.Run( command );
            if ( result.RunResultType == RunResultType.Error )
            {
                System.Console.WriteLine( "Sentence is invalid." );
                System.Console.WriteLine( $"Sentence: {result.Sentence}" );
                System.Console.WriteLine( $"Location: {result.Error!.InvalidSymbolIndex}" );

                continue;
            }

            System.Console.WriteLine( "Sentence is correct" );
        }
    }
}