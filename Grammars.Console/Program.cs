using Grammar.Parsers;
using Grammar.Parsers.Implementation;
using Grammars.Common.Convertors;
using Grammars.Common.Runners.Results;
using Grammars.LL.Convertors;
using Grammars.LL.Models;
using Grammars.LL.Runners;
using Grammars.Visualization;
using Logging;
using Grammars.LL.Visualizations;
using Logging.Implementation;

namespace Grammars.Console;

public class Program
{
    private static readonly ILogger _logger = new ConsoleLogger()
    {
        MinimalLogLevel = LogLevel.None
    };

    private static readonly string _exitCommand = "exit";

    public static void Main()
    {
        LlOneGrammar grammar = BuildGrammar();
        var runner = new LlOneGrammarRunner( grammar, _logger );
        
        ParsingTable table = new ParsingTableCreator().Create( grammar );
        table.ToConsole( grammar );
        
        AskForSentences( runner );
    }

    private static LlOneGrammar BuildGrammar()
    {
        LlOneGrammar grammar = new GrammarFileParser( @"../../../Grammars/common.txt", new ParsingSettings() )
            .Parse()
            .ToConsole( "Original grammar" )
            .Convert( new ToLlOneGrammarConvertor( _logger ) )
            .ToConsole( "LL one grammar" );

        return grammar;
    }

    private static void AskForSentences( LlOneGrammarRunner runner )
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

            RunResult result = runner.Run( command );
            if ( result.RunResultType == RunResultType.Error )
            {
                System.Console.WriteLine( "Sentence is invalid." );
                System.Console.WriteLine( $"Sentence: {result.Sentence}" );

                if ( result.Error!.InvalidSymbolIndex != null )
                {
                    System.Console.WriteLine( $"Location: {result.Error!.InvalidSymbolIndex}" );
                }

                if ( result.Error.Exception is not null )
                {
                    System.Console.WriteLine( $"Unhandled exception. Message: {result.Error.Exception.Message}" );
                }

                continue;
            }

            System.Console.WriteLine( "Sentence is correct" );
        }
    }
}