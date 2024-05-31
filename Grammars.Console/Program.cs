using Grammar.Parsers;
using Grammar.Parsers.Implementation;
using Grammars.Common.Convertors;
using Grammars.Common.Convertors.Convertors;
using Grammars.LL.Convertors;
using Grammars.LL.Models;
using Grammars.LL.Runners;
using Grammars.LL.Runners.Results;
using Grammars.Visualization;
using Logging;
using Grammars.LL.Visualizations;
using Logging.Implementation;

namespace Grammars.Console;

public class Program
{
    private static readonly ILogger _logger = new ConsoleLogger();

    private static readonly string _exitCommand = "exit";

    public static void Main()
    {
        LlOneGrammar grammar = BuildGrammar();
        
        ParsingTable table = new ParsingTableCreator().Create( grammar );
        table.ToConsole( grammar );

        AskForSentences( grammar );
    }

    private static LlOneGrammar BuildGrammar()
    {
        LlOneGrammar grammar = new GrammarFileParser( @"../../../Grammars/common.txt", new ParsingSettings() )
            .Parse()
            .Convert( new RemoveWhitespacesConvertor() )
            .ToConsole( "Original grammar" )
            .Convert( new ToLlOneGrammarConvertor( _logger ) )
            .ToConsole( "LL one grammar" );

        return grammar;
    }

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

                if ( result.Error!.InvalidSymbolIndex != null )
                {
                    System.Console.WriteLine( $"Location: {result.Error!.InvalidSymbolIndex}" );
                }

                if ( result.Error.IsNotLlGrammar )
                {
                    System.Console.WriteLine( "Not LL grammar" );
                }

                continue;
            }

            System.Console.WriteLine( "Sentence is correct" );
        }
    }
}