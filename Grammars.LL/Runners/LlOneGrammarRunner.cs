using System.Diagnostics;
using Grammars.Common.Grammars.ValueObjects.Symbols;
using Grammars.LL.Models;
using Grammars.LL.Runners.Results;
using LinqExtensions;

namespace Grammars.LL.Runners;

public class LlOneGrammarRunner
{
    private readonly LlOneGrammar _grammar;
    private readonly ParsingTableCreator _tableCreator;

    public LlOneGrammarRunner( LlOneGrammar grammar )
    {
        _grammar = grammar;
        _tableCreator = new ParsingTableCreator();
    }

    public RunResult Run( string sentence )
    {
        ParsingTable table = _tableCreator.Create( _grammar );

        var wordsQueue = ParseSentence( sentence );
        
        var stack = new Stack<RuleSymbol>();
        stack.Push( RuleSymbol.TerminalSymbol( TerminalSymbol.End() ) );
        stack.Push( RuleSymbol.NonTerminalSymbol( table.StartRule ) );

        int i = 0;
        while ( wordsQueue.Any() && stack.Any() )
        {
            TerminalSymbol word = wordsQueue.First();
            if ( !table.ContainsKey( word ) )
            {
                return RunResult.Fail( sentence, RunError.InvalidSentence( i ) );
            }

            RuleSymbol currentStackItem = stack.Pop();
            
            if ( currentStackItem.Type == RuleSymbolType.NonTerminalSymbol )
            {
                if ( !table[word].ContainsKey( currentStackItem.RuleName! ) )
                {
                    return RunResult.Fail( sentence, RunError.InvalidSentence( i ) );
                }
                
                foreach ( RuleSymbol ruleSymbol in table[word][currentStackItem.RuleName!].Symbols
                             .Where( x => x.Type != RuleSymbolType.TerminalSymbol || x.Symbol.Type != TerminalSymbolType.End )
                             .Reverse() )
                {
                    stack.Push( ruleSymbol );
                }

                continue;
            }

            if ( currentStackItem.Symbol!.Type == TerminalSymbolType.EmptySymbol )
            {
                continue;
            }

            if ( currentStackItem.Symbol != word )
            {
                throw new UnreachableException();
            }
            
            wordsQueue.RemoveFirst();
            i++;
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