using System.Diagnostics;
using Grammars.Common.Grammars.ValueObjects.Symbols;
using Grammars.Common.Runners;
using Grammars.Common.Runners.Results;
using Grammars.LL.Models;
using Logging;

namespace Grammars.LL.Runners;

public class LlOneGrammarRunner : IGrammarRunner
{
    private readonly ParsingTable _table;
    private readonly ILogger? _logger;

    public LlOneGrammarRunner( LlOneGrammar grammar, ILogger? logger = null )
    {
        _logger = logger;
        _table = new ParsingTableCreator().Create( grammar );
    }

    public RunResult Run( string sentence )
    {
        try
        {
            return RunInternal( sentence );
        }
        catch ( Exception ex )
        {
            _logger?.Write( LogLevel.Error, ex.ToString() );
            return RunResult.Fail( sentence, RunError.UnhandledException( ex ) );
        }
    }

    private RunResult RunInternal( string sentence )
    {
        var wordsQueue = ParseSentence( sentence );
        
        var stack = new Stack<RuleSymbol>();
        stack.Push( RuleSymbol.TerminalSymbol( TerminalSymbol.End() ) );
        stack.Push( RuleSymbol.NonTerminalSymbol( _table.StartRule ) );

        int i = 0;
        while ( wordsQueue.Any() && stack.Any() )
        {
            TerminalSymbol word = wordsQueue.First();
            if ( !_table.ContainsKey( word ) )
            {
                return RunResult.Fail( sentence, RunError.InvalidSentence( i ) );
            }

            RuleSymbol currentStackItem = stack.Pop();
            _logger?.Write( LogLevel.Debug, $"Popping from stack {currentStackItem}" );
            
            if ( currentStackItem.Type == RuleSymbolType.NonTerminalSymbol )
            {
                if ( !_table[word].ContainsKey( currentStackItem.RuleName! ) )
                {
                    return RunResult.Fail( sentence, RunError.InvalidSentence( i ) );
                }
                
                foreach ( RuleSymbol ruleSymbol in _table[word][currentStackItem.RuleName!].Symbols
                             .Where( x => x.Type != RuleSymbolType.TerminalSymbol || x.Symbol!.Type != TerminalSymbolType.End )
                             .Reverse() )
                {
                    stack.Push( ruleSymbol );
                    _logger?.Write( LogLevel.Debug, $"Pushing to stack {ruleSymbol}" );
                }

                continue;
            }

            if ( currentStackItem.Symbol!.Type == TerminalSymbolType.EmptySymbol )
            {
                _logger?.Write( LogLevel.Debug, "Found epsilon" );
                continue;
            }

            if ( currentStackItem.Symbol != word )
            {
                throw new UnreachableException();
            }
            
            wordsQueue.RemoveFirst();
            _logger?.Write( LogLevel.Debug, $"Removing from words queue: {word}" );
            i += word.ToString().Length;
        }

        if ( wordsQueue.Any() && !stack.Any() || 
             !wordsQueue.Any() && stack.Any() )
        {
            return RunResult.Fail( sentence, RunError.InvalidSentence( i ) );
        }
        
        return RunResult.Ok( sentence );
    }

    private LinkedList<TerminalSymbol> ParseSentence( string sentence )
    {
        sentence = sentence.Trim();

        while ( sentence.Contains( "  " ) )
        {
            sentence = sentence.Replace( "  ", " " );
        }

        var result = new LinkedList<TerminalSymbol>();

        IEnumerable<string> parts = sentence.Split( ' ' ).Where( x => x.Length > 0 );
        foreach ( string part in parts )
        {
            result.AddLast( TerminalSymbol.Word( part ) );
        }

        result.AddLast( TerminalSymbol.End() );

        return result;
    }
}